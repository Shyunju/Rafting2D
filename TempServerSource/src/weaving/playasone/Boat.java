package weaving.playasone;

import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.entities.data.SFSObject;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;
import java.util.concurrent.Executors;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.TimeUnit;

public class Boat {

    // 물리 상태 변수
    private float x, y, rotation;
    private float linearVelocityX, linearVelocityY;
    private float angularVelocity; // 각속도

    // 보트 물리 특성
    private final float force = 5f;
    private final float rotateAngle = 2.0f;
//    private final float duration = 1.0f; // 힘이 적용되는 시간
    private final float damping = 0.98f; // 속도 감쇠 계수

    // 동시 입력을 처리하기 위한 변수
    private final List<Integer> pendingDirections = Collections.synchronizedList(new ArrayList<>());
    private final ScheduledExecutorService inputCollector;
    private boolean isProcessingInput = false;

//    private final GameRoomExtension extension;

    public Boat(GameRoomExtension extension) {
//        this.extension = extension;
        this.x = -3f;
        this.y = -8.5f;
        this.rotation = 0;
        this.linearVelocityX = 0;
        this.linearVelocityY = 0;
        this.angularVelocity = 0;

        // 입력 처리를 위한 스케줄러 설정
        this.inputCollector = Executors.newSingleThreadScheduledExecutor();
    }

    // 클라이언트로부터 받은 입력을 처리
    public void processInput(int dir) {
        pendingDirections.add(dir);
        if (!isProcessingInput) {
            isProcessingInput = true;
            // 0.1초 후에 입력들을 종합하여 처리
            inputCollector.schedule(this::applyPendingInputs, 100, TimeUnit.MILLISECONDS);
        }
    }

    private void applyPendingInputs() {
        int totalDir = 0;
        int inputCount = pendingDirections.size();

        if (inputCount == 0) {
            isProcessingInput = false;
            return;
        }

        for (int dir : pendingDirections) {
            totalDir += dir;
        }
        pendingDirections.clear();
        isProcessingInput = false;

        // C#의 MoveAndRotateCo 로직을 기반으로 힘과 회전을 적용
        float rotationAmount = totalDir * rotateAngle;
//        if (inputCount > 1 && totalDir == 0) {
//            rotationAmount += 5.0f; // A, D 동시 입력 시 회전
//        }

        // 실제 물리 업데이트는 update() 메서드에서 처리되므로, 여기서는 목표 속도/회전만 설정
        // 여기서는 간단하게 각속도와 선속도에 직접 힘을 가하는 방식으로 구현
        
        // 목표 회전 적용 (각속도 사용)
        this.angularVelocity += rotationAmount * 0.1f; // 값을 조절하여 회전 속도 변경

        // 목표 속도 적용 (선속도 사용)
        float targetSpeed = force * inputCount;
        float rad = (float) Math.toRadians(this.rotation);
        this.linearVelocityX += (float) Math.cos(rad) * targetSpeed * 0.1f;
        this.linearVelocityY += (float) Math.sin(rad) * targetSpeed * 0.1f;
    }

    // 게임 루프에서 호출될 업데이트 메서드
    public void update(float deltaTime) {
        // 각속도에 따라 회전 업데이트
        this.rotation += this.angularVelocity * deltaTime;

        // 선속도에 따라 위치 업데이트
        this.x += this.linearVelocityX * deltaTime;
        this.y += this.linearVelocityY * deltaTime;

        // 시간이 지남에 따라 속도와 회전력을 점차 줄임 (감쇠)
        this.linearVelocityX *= damping;
        this.linearVelocityY *= damping;
        this.angularVelocity *= damping;
    }

    // 현재 보트의 상태를 SFSObject로 변환
    public ISFSObject toSFSObject() {
        ISFSObject boatData = new SFSObject();
        boatData.putFloat("x", this.x);
        boatData.putFloat("y", this.y);
        boatData.putFloat("rot", this.rotation);
        boatData.putFloat("velX", this.linearVelocityX);
        boatData.putFloat("velY", this.linearVelocityY);
        boatData.putFloat("angVel", this.angularVelocity);
        return boatData;
    }
    
    public void shutdown() {
        inputCollector.shutdownNow();
    }
}
