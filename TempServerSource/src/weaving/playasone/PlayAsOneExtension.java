package weaving.playasone;

import com.smartfoxserver.v2.core.SFSEventType;
import com.smartfoxserver.v2.extensions.ExtensionLogLevel;
import com.smartfoxserver.v2.extensions.SFSExtension;

public class PlayAsOneExtension extends SFSExtension {

	@Override
	public void init(){
		trace("PlayAsOneExtension (Zone Level) initializing...");

		// Zone Level Event Handlers
	    addEventHandler(SFSEventType.USER_LOGIN, LoginHandler.class);
		addEventHandler(SFSEventType.USER_JOIN_ROOM, JoinRoomHandler.class);
		addEventHandler(SFSEventType.USER_DISCONNECT, DisconnectHandler.class);
	    addEventHandler(SFSEventType.USER_VARIABLES_UPDATE, UserVariablesHandler.class);
	    addEventHandler(SFSEventType.ROOM_VARIABLES_UPDATE, RoomVariablesHandler.class);
    
	    // Zone Level Request Handlers
	    addRequestHandler(ConstantClass.KEEPALIVE, PingPongHandler.class);
	    addRequestHandler(ConstantClass.FINDANDJOIN, FindAndJoinRoomHandler.class);
        addRequestHandler(ConstantClass.START_GAME_REQUEST, StartGameRequestHandler.class);
	    addRequestHandler(ConstantClass.ROOMREMOVE, RemoveRoomHandler.class);
	    
	    trace("PlayAsOneExtension (Zone Level) initialized successfully.");
	}

	public void LogMessage(ExtensionLogLevel level, String message)
	{
		// Zone 레벨에서는 특정 룸 컨텍스트 없이 로그를 남깁니다.
		trace(level, message);
	}
	
	public void LogMessage(String message)
	{
		LogMessage(ExtensionLogLevel.INFO, message);
	}
	
	@Override
	public void destroy()
	{
	    trace("PlayAsOneExtension (Zone Level) destroying...");
	    super.destroy();
	}

	// 아래 메서드들은 Room-level extension에 특화된 것들이므로 Zone-level에서는 무의미합니다.
	// 필요시 다른 방식으로 구현해야 합니다.

	/*
	private String getUniqueKey(User user)
	{
		return (String) user.getSession().getProperty("uniqueKey");
	}
	*/
}
