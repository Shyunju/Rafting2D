package weaving.playasone;

import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.entities.data.SFSArray;
import com.smartfoxserver.v2.entities.data.SFSObject;
import com.smartfoxserver.v2.entities.variables.RoomVariable;
import com.smartfoxserver.v2.entities.variables.SFSRoomVariable;
import com.smartfoxserver.v2.extensions.BaseClientRequestHandler;
import com.smartfoxserver.v2.persistence.room.FileRoomStorage;

import java.util.ArrayList;
import java.util.List;
import java.util.Random;

public class GenerateMapRequestHandler extends BaseClientRequestHandler {

    @Override
    public void handleClientRequest(User user, ISFSObject params) {
        
        // --- 가정된 맵 설정 ---
        int rockCount = 20;
        float mapWidth = 20.0f; // X-axis range: -10 to 10
        float mapHeightStart = 10.0f;
        float mapHeightEnd = 100.0f;
        int rockTypes = 3;

        Random random = new Random();
        SFSArray mapData = new SFSArray();

        trace("Generating map data for " + rockCount + " rocks...");

        for (int i = 0; i < rockCount; i++) {
            ISFSObject rockData = new SFSObject();

            // 랜덤 위치 및 타입 생성
            float x = (random.nextFloat() * mapWidth) - (mapWidth / 2);
            float y = mapHeightStart + (random.nextFloat() * (mapHeightEnd - mapHeightStart));
            int type = random.nextInt(rockTypes);

            rockData.putInt("type", type);
            rockData.putFloat("x", x);
            rockData.putFloat("y", y);
            
            mapData.addSFSObject(rockData);
        }

        // 현재 룸에 맵 데이터를 RoomVariable로 저장
        var room = this.getParentExtension().getParentRoom();
        
        // 룸 변수 생성. private이 아니며, 룸에 있는 모든 유저에게 전송됩니다.
        RoomVariable mapVar = new SFSRoomVariable(ConstantClass.MAP_DATA, mapData);
        mapVar.setPrivate(false); 
        
        List<RoomVariable> roomVariables = new ArrayList<>();
        roomVariables.add(mapVar);

        // 룸 변수를 설정합니다. 마지막 파라미터는 서버 이벤트(onRoomVariablesUpdate)를 발생시킬지 여부입니다.
        // true로 설정하면 모든 클라이언트에게 ROOM_VARIABLES_UPDATE 이벤트가 전송됩니다.
        getApi().setRoomVariables(user, room, roomVariables);

        trace("Map data generated and set as RoomVariable for room: " + room.getName());
    }
}
