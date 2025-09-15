package weaving.playasone;

import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.entities.data.SFSObject;
import com.smartfoxserver.v2.extensions.SFSExtension;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collections;
import java.util.List;

public class BoatAIController implements Runnable {

    private final SFSExtension extension;

    public BoatAIController(SFSExtension extension) {
        this.extension = extension;
    }

    @Override
    public void run() {
        try {
            // 4개의 패들 중 2개를 무작위로 선택합니다.
            List<Integer> availablePaddles = new ArrayList<>(Arrays.asList(0, 1, 2, 3));
            Collections.shuffle(availablePaddles); // 리스트를 섞습니다.

            // 첫 두 개의 패들을 선택합니다.
            for (int i = 0; i < 2; i++) {
                int paddleIndex = availablePaddles.get(i);

                // 패들 인덱스에 따라 방향 결정 (짝수: -1, 홀수: 1)
                int direction = (paddleIndex % 2 == 0) ? -1 : 1;

                // 클라이언트에 보낼 데이터 생성
                ISFSObject data = new SFSObject();
                data.putInt("pIdx", paddleIndex);
                data.putInt("dir", direction);

                // 현재 룸의 모든 유저에게 PADDLE_AI 명령 전송
                List<User> userList = extension.getParentRoom().getUserList();
                extension.send(ConstantClass.PADDLE_AI, data, userList);

//                extension.trace(String.format("AI Paddle: index=%d, direction=%d", paddleIndex, direction));
            }
        } catch (Exception e) {
            extension.trace("Error in BoatAIController task: " + e.getMessage());
        }
    }
}
