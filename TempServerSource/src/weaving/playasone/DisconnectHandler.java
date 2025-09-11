package weaving.playasone;

import com.smartfoxserver.v2.core.ISFSEvent;
import com.smartfoxserver.v2.core.SFSEventParam;
import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.exceptions.SFSException;
import com.smartfoxserver.v2.extensions.BaseServerEventHandler;

public class DisconnectHandler extends BaseServerEventHandler {

    @Override
    public void handleServerEvent(ISFSEvent event) throws SFSException {
        // USER_DISCONNECT 이벤트로부터 접속 종료한 User 객체를 얻습니다.
        User user = (User) event.getParameter(SFSEventParam.USER);
        trace(String.format("User '%s' (ID: %d) has disconnected from the Zone.", user.getName(), user.getId()));

        // 만약을 위해, 유저가 마지막으로 접속했던 룸에서 수동으로 나가도록 처리합니다.
        // SFS2X가 이 처리를 자동으로 하지만, 현재 문제를 해결하기 위해 명시적으로 코드를 추가합니다.
        if (user.getLastJoinedRoom() != null) {
            try {
                getApi().leaveRoom(user, user.getLastJoinedRoom());
                trace(String.format("Manually removed user '%s' from room '%s'.", user.getName(), user.getLastJoinedRoom().getName()));
            } catch (Exception e) {
                trace("Error manually leaving room on disconnect: " + e.getMessage());
            }
        }
    }
}
