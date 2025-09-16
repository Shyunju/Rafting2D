package weaving.playasone;

import com.smartfoxserver.v2.entities.Room;
import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.entities.data.SFSObject;
import com.smartfoxserver.v2.extensions.BaseClientRequestHandler;

public class PaddleRequestHandler extends BaseClientRequestHandler {

    @Override
    public void handleClientRequest(User sender, ISFSObject params) {
        int dir = params.getInt("dir");
        int pIdx = params.getInt("pidx");

        Room currentRoom = sender.getLastJoinedRoom();
        if (currentRoom == null) {
            trace("User " + sender.getName() + " is not in a room.");
            return;
        }

        // 확장 가져오기
        GameRoomExtension extension = (GameRoomExtension) getParentExtension();
        // Get the player's boat using its ID
        Boat boat = extension.getBoat("playerBoat"); // Use the fixed ID for the player boat

        if (boat != null) {
            // 보트 객체에 입력 전달
            boat.processInput(dir);
        }

        // 다른 모든 클라이언트에게 애니메이션을 재생하도록 알림
        ISFSObject animationResponse = new SFSObject();
        animationResponse.putInt("pIdx", pIdx);
//        send(ConstantClass.PADDLE_ANIMATION, animationResponse, currentRoom.getUsersInRoomBut(sender));
        send(ConstantClass.PADDLE_ANIMATION, animationResponse, currentRoom.getUserList());
    }
}
