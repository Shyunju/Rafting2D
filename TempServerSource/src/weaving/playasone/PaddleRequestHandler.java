package weaving.playasone;

import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.entities.data.SFSObject;
import com.smartfoxserver.v2.extensions.BaseClientRequestHandler;

public class PaddleRequestHandler extends BaseClientRequestHandler {

    @Override
    public void handleClientRequest(User sender, ISFSObject params) {
        // Get the direction from the client
        int dir = params.getInt("dir");
        trace(String.format("User %s paddled with direction: %d", sender.getName(), dir));

        // For now, just send a paddle animation trigger back to all users in the room
        // In a real game, this would involve game logic to update boat state
        // and then send the updated state to all clients.
        ISFSObject response = new SFSObject();
        // Assuming 'pIdx' is the paddle index, which needs to be determined by server logic
        // For now, let's just send a dummy index or derive it from the sender/dir
        // For simplicity, let's just send the sender's ID as a placeholder for paddle index
        response.putInt("pIdx", sender.getId()); 

        // Send the response to all users in the sender's room
        send(ConstantClass.PADDLE_ANIMATION, response, sender.getLastJoinedRoom().getUserList());
    }
}
