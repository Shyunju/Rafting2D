package weaving.playasone;

import com.smartfoxserver.v2.api.CreateRoomSettings;
import com.smartfoxserver.v2.entities.Room;
import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.exceptions.SFSCreateRoomException;
import com.smartfoxserver.v2.exceptions.SFSJoinRoomException;
import com.smartfoxserver.v2.extensions.BaseClientRequestHandler;

import java.util.List;

public class FindAndJoinRoomHandler extends BaseClientRequestHandler {

    @Override
    public void handleClientRequest(User user, ISFSObject params) {
        // Get the list of rooms in the current zone
        List<Room> roomList = getParentExtension().getParentZone().getRoomList();
        Room targetRoom = null;

        // "대기중" 상태이고, 꽉 차지 않은 게임 룸을 찾습니다.
        for (Room room : roomList) {
            if (room.isGame() && !room.isFull()) {
                // 방의 상태 변수를 확인합니다.
                com.smartfoxserver.v2.entities.variables.RoomVariable stateVar = room.getVariable(ConstantClass.ROOM_STATE);
                if (stateVar != null && stateVar.getStringValue().equals(ConstantClass.STATE_WAITING)) {
                    targetRoom = room;
                    break; // Found a room, exit the loop
                }
            }
        }

        try {
            if (targetRoom != null) {
                // If a suitable room is found, join it
                getApi().joinRoom(user, targetRoom);
            } else {
                // If no suitable room is found, create a new one
                CreateRoomSettings settings = new CreateRoomSettings();
                // Generate a unique name for the room
                String roomName = "GR_" + System.currentTimeMillis();
                settings.setName(roomName);
                settings.setMaxUsers(4); // Set max users to 4
                settings.setGame(true);
                settings.setDynamic(true); // 동적 룸으로 설정하여 자동 삭제가 가능하도록 합니다.

                // 방 생성 시 "대기중" 상태와 "방장 ID"를 Room Variable로 설정합니다.
                java.util.List<com.smartfoxserver.v2.entities.variables.RoomVariable> roomVariables = new java.util.ArrayList<>();
                
                // 1. 게임 상태 변수 추가
                com.smartfoxserver.v2.entities.variables.SFSRoomVariable roomState = new com.smartfoxserver.v2.entities.variables.SFSRoomVariable(ConstantClass.ROOM_STATE, ConstantClass.STATE_WAITING);
                roomState.setGlobal(true); // Global 변수로 설정하여 룸 리스트에서 조회 가능하도록 합니다.
                roomVariables.add(roomState);

                // 2. 방장 ID 변수 추가
                com.smartfoxserver.v2.entities.variables.SFSRoomVariable ownerIdVar = new com.smartfoxserver.v2.entities.variables.SFSRoomVariable(ConstantClass.ROOM_OWNER_ID, user.getId());
                ownerIdVar.setGlobal(true);
                roomVariables.add(ownerIdVar);

                settings.setRoomVariables(roomVariables);

                settings.setGroupId("default"); // Or any other group you use
                // 방이 비었을 때 자동으로 파괴되도록 설정합니다.
                settings.setAutoRemoveMode(com.smartfoxserver.v2.entities.SFSRoomRemoveMode.WHEN_EMPTY);

                // Set the Room Extension for this room
                settings.setExtension("weaving-playasone", "weaving.playasone.RaftingRoomExtension");

                // Create the room and join the user
                // 네 번째 인자(joinIt)를 true로 설정하여 방 생성자가 즉시 입장하도록 합니다.
                getApi().createRoom(getParentExtension().getParentZone(), settings, user, true, null, true, true);
            }
        } catch (SFSJoinRoomException | SFSCreateRoomException e) {
            trace(e.getMessage());
            // Optionally, send an error message back to the client
            ISFSObject errorParams = new com.smartfoxserver.v2.entities.data.SFSObject();
            errorParams.putUtfString("error", "Failed to join or create room: " + e.getMessage());
            send(ErrorClass.ROOM_NOT_AVAILABLE, errorParams, user);
        }
    }
}
