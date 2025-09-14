package weaving.playasone;

public class ConstantClass {
	
	public static final String GAME_TYPE            = "10";

	//ROOM VARIABLE
	public static final String ROOMNAME          	= "20";
	public static final String ISPUBLIC          	= "21";
	public static final String MAXUSERCNT          	= "22";
	public static final String ROOMREMOVE          	= "23";
	public static final String FINDANDJOIN         	= "24";
	
	//ROOM STATE
	public static final String ROOM_OWNER_ID 		= "25";
	public static final String ROOM_STATE 			= "26";
	public static final String STATE_WAITING 		= "27";
	public static final String STATE_PLAYING 		= "28";	

	//GAMEPLAY REQUESTS
	public static final String START_GAME_REQUEST 	= "30";
	public static final String COUNTDOWN_RESPONSE 	= "30";
	
	
	// Rafting Game
	public static final String PADDLE_REQUEST 		= "40";
	public static final String PADDLE_ANIMATION 	= "41"; 
	public static final String PADDLE_AI 			= "42"; 

	// WAITING ROOM
	public static final String UPDATE_USER_LIST 	= "50"; 

	// MAP GENERATION
	public static final String GENERATE_MAP_REQUEST = "60";
	public static final String MAP_DATA 			= "61";
	
	//HANDLER
	public static final String KEEPALIVE			= "1000";
	
	//ERROR
	public static final String ERROR				= "9990";
	public static final String SERVER_MESSAGE		= "9991";
	public static final String ERROR_CODE			= "9998";
	public static final String ERROR_MESSAGE		= "9999";
}