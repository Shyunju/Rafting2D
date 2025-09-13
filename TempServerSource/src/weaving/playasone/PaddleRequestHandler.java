package weaving.playasone;

import com.smartfoxserver.v2.entities.Room;
import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.entities.data.SFSObject;
import com.smartfoxserver.v2.extensions.BaseClientRequestHandler;


public class PaddleRequestHandler extends BaseClientRequestHandler {

//    private static final float MOVE_STEP = 0.1f; // Define a small movement step

    @Override
    public void handleClientRequest(User sender, ISFSObject params) {
        int dir = params.getInt("dir");
        int pIdx = params.getInt("pidx");

        Room currentRoom = sender.getLastJoinedRoom();
        if (currentRoom == null) {
            trace("User " + sender.getName() + " is not in a room.");
            return;
        }

        // Send PADDLE_ANIMATION
        ISFSObject animationResponse = new SFSObject();
        animationResponse.putInt("pIdx", pIdx); 
        animationResponse.putInt("dir", dir); 
        send(ConstantClass.PADDLE_ANIMATION, animationResponse, currentRoom.getUserList());

    }
}
