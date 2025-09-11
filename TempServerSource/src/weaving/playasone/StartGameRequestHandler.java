package weaving.playasone;

import com.smartfoxserver.v2.entities.Room;
import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.entities.variables.SFSRoomVariable;
import com.smartfoxserver.v2.extensions.BaseClientRequestHandler;

import java.util.ArrayList;
import java.util.List;

public class StartGameRequestHandler extends BaseClientRequestHandler {

    @Override
    public void handleClientRequest(User user, ISFSObject params) {
        // 유저가 현재 있는 방을 가져옵니다.
        Room currentRoom = user.getLastJoinedRoom();
        if (currentRoom == null) {
            trace("User " + user.getName() + " is not in a room.");
            return;
        }

        // 방의 상태를 "playing"으로 변경합니다.
        List<com.smartfoxserver.v2.entities.variables.RoomVariable> roomVariables = new ArrayList<>();
        SFSRoomVariable roomState = new SFSRoomVariable(ConstantClass.ROOM_STATE, ConstantClass.STATE_PLAYING);
        roomVariables.add(roomState);

        getApi().setRoomVariables(user, currentRoom, roomVariables);

        

        trace(String.format("Room '%s' state changed to PLAYING by user '%s'.", currentRoom.getName(), user.getName()));
    }
}