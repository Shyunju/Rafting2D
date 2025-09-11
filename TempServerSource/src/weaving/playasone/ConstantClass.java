package weaving.playasone;

public class ConstantClass {
	
	// Character
	public static final String X                    = "19";
	public static final String Y                    = "20";
	public static final String Z                    = "21";

	
	//ROOM VARIABLE
	public static final String ROOMNAME          	= "40";
	public static final String ISPUBLIC          	= "41";
	public static final String MAXUSERCNT          	= "42";
	public static final String ROOMREMOVE          	= "43";
	public static final String FINDANDJOIN         	= "44";

	//ROOM STATE
	public static final String ROOM_OWNER_ID 		= "49";
	public static final String ROOM_STATE 			= "45";
	public static final String STATE_WAITING 		= "46";
	public static final String STATE_PLAYING 		= "47";

	//GAMEPLAY REQUESTS
	public static final String START_GAME_REQUEST 	= "48";
	public static final String PADDLE_REQUEST 		= "105"; // New: Paddle input request

	// SERVER RESPONSES / GAME STATE UPDATES
	public static final String GAME_STATE_UPDATE 	= "201"; // New: Game state update from server
	public static final String PADDLE_ANIMATION 	= "202"; // New: Paddle animation trigger from server
	
	//HANDLER
	public static final String KEEPALIVE			= "1000";
	
	//ERROR
	public static final String ERROR				= "9990";
	public static final String SERVER_MESSAGE		= "9991";
	public static final String ERROR_CODE			= "9998";
	public static final String ERROR_MESSAGE		= "9999";
}