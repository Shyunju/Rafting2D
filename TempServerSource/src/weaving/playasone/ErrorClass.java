package weaving.playasone;

import java.util.ArrayList;

public class ErrorClass {
	public static final String ALLOK 				= "E000";
	
	public static final String GAME_SERVER_FULL 	= "E700";
	public static final String ROOM_NOT_AVAILABLE 	= "E701";
	public static final String ROOM_ALREADY_FULL 	= "E702";
	public static final String ROOM_ALREADY_EXIST 	= "E703";

	public static final String DATA_EMPTY 			= "E797";
	public static final String DATA_MISMATCH 		= "E798";
	public static final String SERVER_UNKNOWN_ERROR = "E799";
	
	/*
	 * Error Message in Array
	 * Index 0 -> English
	 * Index 1 -> Korean
	 */
	public static ArrayList<String> GetErrorMessage(String ErrorCode)
	{
		ArrayList<String> temp = new ArrayList<String>();
		
		switch (ErrorCode)
		{
			case "E000":
				temp.add("OK");
				temp.add("-");
				break;
			case "E700":
				temp.add("Unknown Error has been occured.");
				temp.add("-");
				break;
			case "E701":
				temp.add("The room is not available. Please try to enter another room.");
				temp.add("-");
				break;
			case "E702":
				temp.add("The room is already full. Please try to enter another room.");
				temp.add("-");
				break;
			case "E703":
				temp.add("The room already exist.");
				temp.add("-");
				break;
			case "E797":
				temp.add("Data search result is empty.");
				temp.add("-");
				break;
			case "E798":
				temp.add("Some wrong data have been found. You need to restart the game.");
				temp.add("-");
				break;
			case "E799":
				temp.add("Unknown Error has been occured.");
				temp.add("-");
				break;
			default:
				temp.add("-");
				temp.add("-");
				break;
		}
		
		return temp;
	}
}
