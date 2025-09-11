package weaving.playasone;

import com.smartfoxserver.v2.entities.Room;
import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.entities.data.SFSObject;
import com.smartfoxserver.v2.entities.variables.RoomVariable;
import com.smartfoxserver.v2.entities.variables.SFSRoomVariable;
import com.smartfoxserver.v2.extensions.BaseClientRequestHandler;

import java.util.ArrayList;
import java.util.List;

public class PaddleRequestHandler extends BaseClientRequestHandler {

    private static final float MOVE_STEP = 0.1f; // Define a small movement step

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
        send(ConstantClass.PADDLE_ANIMATION, animationResponse, currentRoom.getUserList());
        trace(String.format("User %s paddled with direction: %d, sending PADDLE_ANIMATION for pIdx: %d", sender.getName(), dir, pIdx));


        // Get current boat position from Room Variables
        RoomVariable varPosX = currentRoom.getVariable(ConstantClass.BOAT_POS_X);
        RoomVariable varPosY = currentRoom.getVariable(ConstantClass.BOAT_POS_Y);
        RoomVariable varPosZ = currentRoom.getVariable(ConstantClass.BOAT_POS_Z);

        float currentPosX = (varPosX != null) ? ((Double) varPosX.getValue()).floatValue() : 0.0f;
        float currentPosY = (varPosY != null) ? ((Double) varPosY.getValue()).floatValue() : 0.0f;
        float currentPosZ = (varPosZ != null) ? ((Double) varPosZ.getValue()).floatValue() : 0.0f;

        // Calculate new position based on direction
        float newPosX = currentPosX;
        float newPosY = currentPosY;
        float newPosZ = currentPosZ;

        if (dir == 1) { // Move forward
            newPosZ += MOVE_STEP;
        } else if (dir == -1) { // Move backward
            newPosZ -= MOVE_STEP;
        }
        // Add more complex movement logic (e.g., X-axis, rotation) if needed

        // Update Room Variables with new boat position
        List<RoomVariable> roomVariables = new ArrayList<>();
        roomVariables.add(new SFSRoomVariable(ConstantClass.BOAT_POS_X, newPosX, true, true, false)); // last true for global
        roomVariables.add(new SFSRoomVariable(ConstantClass.BOAT_POS_Y, newPosY, true, true, false));
        roomVariables.add(new SFSRoomVariable(ConstantClass.BOAT_POS_Z, newPosZ, true, true, false));
        
        getApi().setRoomVariables(sender, currentRoom, roomVariables);

        // 5. Send GAME_STATE_UPDATE to all clients in the room
        ISFSObject gameStateUpdateResponse = new SFSObject();
        gameStateUpdateResponse.putFloat(ConstantClass.BOAT_POS_X, newPosX);
        gameStateUpdateResponse.putFloat(ConstantClass.BOAT_POS_Y, newPosY);
        gameStateUpdateResponse.putFloat(ConstantClass.BOAT_POS_Z, newPosZ);

        // Send to all users in the room
        send(ConstantClass.GAME_STATE_UPDATE, gameStateUpdateResponse, currentRoom.getUserList());

        trace(String.format("Boat position updated to X:%.2f, Y:%.2f, Z:%.2f by user %s, sending GAME_STATE_UPDATE", newPosX, newPosY, newPosZ, sender.getName()));
    }
}
