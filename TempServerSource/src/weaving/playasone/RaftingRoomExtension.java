package weaving.playasone;

import com.smartfoxserver.v2.core.SFSEventType;
import com.smartfoxserver.v2.extensions.SFSRoomExtension;

public class RaftingRoomExtension extends SFSRoomExtension {

    @Override
    public void init() {
        trace("RaftingRoomExtension (Room Level) initializing for Room: " + getParentRoom().getName());

        // Add Room Level Request Handlers
        addRequestHandler(ConstantClass.PADDLE_REQUEST, PaddleRequestHandler.class);

        // Add Room Level Event Handlers (if any)
        // For example, to handle user leaving the room
        // addEventHandler(SFSEventType.USER_LEAVE_ROOM, UserLeaveRoomHandler.class);

        trace("RaftingRoomExtension (Room Level) initialized successfully for Room: " + getParentRoom().getName());
    }

    @Override
    public void destroy() {
        trace("RaftingRoomExtension (Room Level) destroying for Room: " + getParentRoom().getName());
        super.destroy();
    }
}
