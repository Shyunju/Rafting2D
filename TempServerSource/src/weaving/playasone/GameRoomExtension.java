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
	
	@Override
	public void init() {
		this.room = getParentRoom();
		trace("GameRoomExtension (Room Level) initializing for Room: " + room.getName());
		
		// 게임 플레이 중의 요청을 처리할 핸들러들을 등록합니다.
		addRequestHandler(ConstantClass.PADDLE_REQUEST, PaddleRequestHandler.class);
		
		addEventHandler(SFSEventType.ROOM_VARIABLES_UPDATE, (event) -> {
		@SuppressWarnings("unchecked")
		List<RoomVariable> changedVars = (List<RoomVariable>) event.getParameter(SFSEventParam.VARIABLES);
		
		// 변경된 변수 목록을 순회합니다.
		for (RoomVariable var : changedVars) {
			// 변수 이름이 ROOM_STATE와 일치하는지 확인합니다.
			if (var.getName().equals(ConstantClass.ROOM_STATE)) {
				// 변수의 새로운 값이 STATE_PLAYING과 일치하는지 확인합니다.
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
		if (countdownTask != null) {
			countdownTask.cancel(true);
		}
		super.destroy();
	}
	
	/**
	 * 게임 시작 카운트다운을 시작하고, 방 안의 모든 유저에게 숫자를 전송합니다.
	*/
	public void startCountdown() {
		if (countdownTask != null && !countdownTask.isDone()) {
			trace("Countdown is already in progress.");
			return;
		}
		
		trace("Starting game countdown for room: " + room.getName());
		AtomicInteger countdownValue = new AtomicInteger(5); // 5초 카운트다운
		
		countdownTask = SmartFoxServer.getInstance().getTaskScheduler().scheduleAtFixedRate(() -> {
			int count = countdownValue.getAndDecrement();
			ISFSObject response = new SFSObject();
			
			if (count > 0) {
				// 카운트다운 숫자 전송
				response.putInt("count", count);
				send(ConstantClass.COUNTDOWN_RESPONSE, response, room.getUserList());
				trace("Countdown: " + count);
			} else if (count == 0) {
				// 게임 시작 메시지 전송
				response.putText("text", "START!");
				send(ConstantClass.COUNTDOWN_RESPONSE, response, room.getUserList());
				trace("Countdown: START!");
			} else {
				// 카운트다운 종료 및 태스크 중지
//				trace("Countdown finished.");
				countdownTask.cancel(false);
			}
		}, 0, 1, TimeUnit.SECONDS);
	}
}