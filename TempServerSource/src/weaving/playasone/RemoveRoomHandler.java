package weaving.playasone;

import com.smartfoxserver.v2.entities.Room;
import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.extensions.BaseClientRequestHandler;

public class RemoveRoomHandler extends BaseClientRequestHandler {

    @Override
    public void handleClientRequest(User user, ISFSObject params) {
        // 클라이언트가 "roomName"이라는 키로 룸 이름을 보내줘야 합니다.
        String roomNameToRemove = params.getUtfString("roomName");

        if (roomNameToRemove == null || roomNameToRemove.isEmpty()) {
            trace("RoomRemoveHandler Error: 'roomName' parameter is missing.");
            return;
        }

        Room room = getParentExtension().getParentZone().getRoomByName(roomNameToRemove);

        if (room != null) {
            getApi().removeRoom(room);
            trace(String.format("Room '%s' was removed by user '%s'.", room.getName(), user.getName()));
        }
        else {
            trace(String.format("RoomRemoveHandler Error: Room '%s' not found.", roomNameToRemove));
        }
    }
}
