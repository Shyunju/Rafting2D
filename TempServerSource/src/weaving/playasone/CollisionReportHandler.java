package weaving.playasone;

import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.extensions.BaseClientRequestHandler;

public class CollisionReportHandler extends BaseClientRequestHandler {

    @Override
    public void handleClientRequest(User sender, ISFSObject params) {
        // trace("CollisionReportHandler: Received report from " + sender.getName());

        // Get the parent extension
        GameRoomExtension extension = (GameRoomExtension) getParentExtension();

        // Extract boatId from the client report
        String boatId = params.getUtfString("boatId");
        // Get the correct boat using its ID
        Boat boat = extension.getBoat(boatId);

        if (boat != null) {
            // Extract collision normal from client report
            float normalX = params.getFloat("normalX");
            float normalY = params.getFloat("normalY");

            // Stop the boat's movement on the server and temporarily ignore input, passing the normal
            boat.stopForCollision(normalX, normalY);

            // Optionally, update the server's boat position to the client's reported position
            // This makes the server state more accurate with where the client actually collided
            float x = params.getFloat("x");
            float y = params.getFloat("y");
            float rot = params.getFloat("rot");
            boat.setPosition(x, y, rot);

            // The game loop will automatically sync this new state to all clients
        }
    }
}
