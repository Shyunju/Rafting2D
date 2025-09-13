package weaving.playasone;

import com.smartfoxserver.v2.SmartFoxServer;
import com.smartfoxserver.v2.entities.Room;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.entities.data.SFSObject;
import com.smartfoxserver.v2.extensions.SFSExtension;

import java.util.concurrent.TimeUnit;
import java.util.concurrent.atomic.AtomicInteger;
import java.util.concurrent.ScheduledFuture;

public class GameRoomExtension extends SFSExtension {
	
    private Room room;
    private ScheduledFuture<?> countdownTask;
    
    @Override
    public void init() {
        trace("GameRoomExtension (Room Level) initializing for Room: " + getParentRoom().getName());
        this.room = getParentRoom();

        // Add Room Level Request Handlers
        addRequestHandler(ConstantClass.PADDLE_REQUEST, PaddleRequestHandler.class);

        // Add Room Level Event Handlers (if any)
        // For example, to handle user leaving the room
        // addEventHandler(SFSEventType.USER_LEAVE_ROOM, UserLeaveRoomHandler.class);

        trace("GameRoomExtension (Room Level) initialized successfully for Room: " + getParentRoom().getName());
    }


    @Override
    public void destroy() {
        // 확장이 소멸될 때 예약된 태스크가 있다면 중지합니다.
        if (countdownTask != null) {
            countdownTask.cancel(true);
        }
        super.destroy();
    }
    
    /**
     * 게임 시작 카운트다운을 시작합니다.
     */
    public void startCountdown() {
        // ScheduledFuture의 isDone 메소드를 사용하여 태스크 완료 여부를 확인합니다.
        if (countdownTask != null && !countdownTask.isDone()) {
            trace("Countdown is already in progress.");
            return;
        }

        trace("Starting game countdown...");
        AtomicInteger countdownValue = new AtomicInteger(5); // 5초 카운트다운

        // scheduleAtFixedRate는 ScheduledFuture<?>를 반환하므로, countdownTask 변수에 할당할 수 있습니다.
        countdownTask =  SmartFoxServer.getInstance().getTaskScheduler().scheduleAtFixedRate(() -> {
            int count = countdownValue.getAndDecrement();
            ISFSObject response = new SFSObject();

            if (count > 0) {
                response.putInt("count", count);
                send(ConstantClass.COUNTDOWN_RESPONSE, response, room.getUserList());
                trace("Countdown: " + count);
            } else if (count == 0) {
                response.putText("text", "START!");
                send(ConstantClass.COUNTDOWN_RESPONSE, response, room.getUserList());
                trace("Countdown: START!");
            } else {
                trace("Countdown finished.");
                countdownTask.cancel(false);
            }

        }, 0, 1, TimeUnit.SECONDS);
    }

}
