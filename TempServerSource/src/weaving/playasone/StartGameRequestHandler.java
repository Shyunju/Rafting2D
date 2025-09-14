package weaving.playasone;

import com.smartfoxserver.v2.entities.Room;
import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.entities.data.SFSArray;
import com.smartfoxserver.v2.entities.data.SFSObject;
import com.smartfoxserver.v2.entities.variables.RoomVariable;
import com.smartfoxserver.v2.entities.variables.SFSRoomVariable;
import com.smartfoxserver.v2.extensions.BaseClientRequestHandler;

import java.util.ArrayList;
import java.util.List;
import java.util.Random;

public class StartGameRequestHandler extends BaseClientRequestHandler {

    @Override
    public void handleClientRequest(User user, ISFSObject params) {
        Room currentRoom = user.getLastJoinedRoom();
        if (currentRoom == null) {
            trace("User " + user.getName() + " is not in a room.");
            return;
        }

        // --- 맵 데이터 생성 로직 시작 ---
        int rockCount = 20;
        float mapWidthStart = 20.0f;
        float mapWidthEnd = 210.0f;
        float mapHeightStart = -20.0f;
        float mapHeightEnd = 30.0f;
        int rockTypes = 3;

        float totalMapWidth = mapWidthEnd - mapWidthStart;
        float laneWidth = totalMapWidth / rockCount;

        Random random = new Random();
        SFSArray mapData = new SFSArray();

        for (int i = 0; i < rockCount; i++) {
            ISFSObject rockData = new SFSObject();

            // 각 세로 구역(Lane) 내에서 X좌표를 랜덤으로 결정합니다.
            float laneStartX = mapWidthStart + (i * laneWidth);
            float x = laneStartX + (random.nextFloat() * laneWidth);

            // Y좌표는 전체 높이 내에서 랜덤으로 결정합니다.
            float y = mapHeightStart + (random.nextFloat() * (mapHeightEnd - mapHeightStart));
            
            int type = random.nextInt(rockTypes);

            rockData.putInt("type", type);
            rockData.putFloat("x", x);
            rockData.putFloat("y", y);
            
            mapData.addSFSObject(rockData);
        }
        // --- 맵 데이터 생성 로직 종료 ---

        // 업데이트할 룸 변수 리스트 생성
        List<RoomVariable> roomVariables = new ArrayList<>();

        // 1. 맵 데이터 변수 추가
        SFSRoomVariable mapDataVar = new SFSRoomVariable(ConstantClass.MAP_DATA, mapData);
        roomVariables.add(mapDataVar);

        // 2. 게임 상태 변수 추가 ("playing")
        SFSRoomVariable roomState = new SFSRoomVariable(ConstantClass.ROOM_STATE, ConstantClass.STATE_PLAYING);
        roomVariables.add(roomState);

        // 3. 게임 타입 변수 추가
        SFSRoomVariable gameTypeVar = new SFSRoomVariable(ConstantClass.GAME_TYPE, "RaftingGame");
        gameTypeVar.setGlobal(true);
        roomVariables.add(gameTypeVar);

        // 생성된 모든 룸 변수를 클라이언트에 전송
        getApi().setRoomVariables(user, currentRoom, roomVariables);

        trace(String.format("Map generated and room '%s' state changed to PLAYING by user '%s'.", currentRoom.getName(), user.getName()));
    }
}