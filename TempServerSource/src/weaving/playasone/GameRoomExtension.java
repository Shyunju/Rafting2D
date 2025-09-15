package weaving.playasone;

import com.smartfoxserver.v2.SmartFoxServer;
import com.smartfoxserver.v2.core.SFSEventParam;
import com.smartfoxserver.v2.core.SFSEventType;
import com.smartfoxserver.v2.entities.Room;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.entities.data.SFSObject;
import com.smartfoxserver.v2.extensions.SFSExtension;
import com.smartfoxserver.v2.entities.variables.RoomVariable;

import java.util.List;
import java.util.concurrent.ScheduledFuture;
import java.util.concurrent.TimeUnit;
import java.util.concurrent.atomic.AtomicInteger;

public class GameRoomExtension extends SFSExtension {
    private Room room;
    private ScheduledFuture<?> countdownTask;
    private ScheduledFuture<?> aiTask;
    private ScheduledFuture<?> gameLoopTask;
    private BoatAIController aiController;
    private Boat boat;

    private static final int AI_LOOP_TIME_SECONDS = 500;
    private static final int GAME_LOOP_INTERVAL_MS = 50; // 20 updates per second
    private long lastUpdateTime;

    @Override
    public void init() {
        this.room = getParentRoom();
        this.boat = new Boat(this);
        trace("GameRoomExtension (Room Level) initializing for Room: " + room.getName());

        addRequestHandler(ConstantClass.PADDLE_REQUEST, PaddleRequestHandler.class);

        addEventHandler(SFSEventType.ROOM_VARIABLES_UPDATE, (event) -> {
            @SuppressWarnings("unchecked")
            List<RoomVariable> changedVars = (List<RoomVariable>) event.getParameter(SFSEventParam.VARIABLES);

            for (RoomVariable var : changedVars) {
                if (var.getName().equals(ConstantClass.ROOM_STATE)) {
                    if (var.getStringValue().equals(ConstantClass.STATE_PLAYING)) {
                        trace("Room state changed to PLAYING. Starting countdown.");
                        startCountdown();
                    }
                    break;
                }
            }
        });

        trace("GameRoomExtension (Room Level) initialized successfully for Room: " + room.getName());
    }

    @Override
    public void destroy() {
        if (countdownTask != null) countdownTask.cancel(true);
        if (aiTask != null) aiTask.cancel(true);
        if (gameLoopTask != null) gameLoopTask.cancel(true);
        if (boat != null) boat.shutdown();
        super.destroy();
    }

    public void startCountdown() {
        if (countdownTask != null && !countdownTask.isDone()) {
            trace("Countdown is already in progress.");
            return;
        }

        trace("Starting game countdown for room: " + room.getName());
        AtomicInteger countdownValue = new AtomicInteger(5);

        countdownTask = SmartFoxServer.getInstance().getTaskScheduler().scheduleAtFixedRate(() -> {
            int count = countdownValue.getAndDecrement();
            ISFSObject response = new SFSObject();

            if (count > 0) {
                response.putInt("count", count);
                send(ConstantClass.COUNTDOWN_RESPONSE, response, room.getUserList());
//                trace("Countdown: " + count);
            } else if (count == 0) {
                response.putText("text", "START!");
                send(ConstantClass.COUNTDOWN_RESPONSE, response, room.getUserList());
//                trace("Countdown: START!");
            } else {
                countdownTask.cancel(false);
                startGameLoop();
                
                // AI 보트 컨트롤러 시작
                this.aiController = new BoatAIController(this);
                this.aiTask = SmartFoxServer.getInstance().getTaskScheduler().scheduleAtFixedRate(this.aiController, 0, AI_LOOP_TIME_SECONDS, TimeUnit.MILLISECONDS);
            }
        }, 0, 1, TimeUnit.SECONDS);
    }

    private void startGameLoop() {
        trace("Starting game loop for room: " + room.getName());
        lastUpdateTime = System.nanoTime();
        gameLoopTask = SmartFoxServer.getInstance().getTaskScheduler().scheduleAtFixedRate(() -> {
            long now = System.nanoTime();
            float deltaTime = (now - lastUpdateTime) / 1_000_000_000.0f;
            lastUpdateTime = now;
            boat.update(deltaTime);

            ISFSObject boatData = boat.toSFSObject();
            send(ConstantClass.BOAT_SYNC, boatData, room.getUserList());
        }, 0, GAME_LOOP_INTERVAL_MS, TimeUnit.MILLISECONDS);
    }

    public Boat getBoat() {
        return this.boat;
    }
}
