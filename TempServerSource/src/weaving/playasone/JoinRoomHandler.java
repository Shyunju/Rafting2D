package weaving.playasone;

import com.smartfoxserver.v2.core.ISFSEvent;
import com.smartfoxserver.v2.core.SFSEventParam;
import com.smartfoxserver.v2.entities.Room;
import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.data.ISFSArray;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.entities.data.SFSArray;
import com.smartfoxserver.v2.entities.data.SFSObject;
import com.smartfoxserver.v2.exceptions.SFSException;
import com.smartfoxserver.v2.extensions.BaseServerEventHandler;

import java.util.List;

public class JoinRoomHandler extends BaseServerEventHandler {

    @Override
    public void handleServerEvent(ISFSEvent event) throws SFSException {
        Room room = (Room) event.getParameter(SFSEventParam.ROOM);
        User user = (User) event.getParameter(SFSEventParam.USER);
        trace(String.format("User '%s' has joined Room '%s'.", user.getName(), room.getName()));

        // 게임 룸에 입장했을 때만 유저 목록을 전송합니다.
        if (!room.isGame()) {
            return;
        }

        // 현재 방의 전체 유저 목록을 가져옵니다.
        List<User> userList = room.getUserList();
        ISFSArray userNames = new SFSArray();

        // 유저 목록에서 이름만 추출하여 SFSArray에 추가합니다.
        for (User u : userList) {
            userNames.addUtfString(u.getName());
        }

        // 전송할 데이터를 SFSObject에 담습니다.
        ISFSObject responseData = new SFSObject();
        responseData.putSFSArray("userList", userNames);

        // 방 안의 모든 유저에게 새로운 유저 목록을 보냅니다.
        send(ConstantClass.UPDATE_USER_LIST, responseData, userList);
        trace(String.format("Sent user list update to %d users in room '%s'.", userList.size(), room.getName()));
    }
}
