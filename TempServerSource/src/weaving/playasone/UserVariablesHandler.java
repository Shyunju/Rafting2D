package weaving.playasone;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

import com.smartfoxserver.v2.extensions.BaseServerEventHandler;
import com.smartfoxserver.v2.mmo.Vec3D;
import com.smartfoxserver.v2.exceptions.SFSException;
import com.smartfoxserver.v2.SmartFoxServer;
import com.smartfoxserver.v2.api.ISFSMMOApi;
import com.smartfoxserver.v2.core.ISFSEvent;
import com.smartfoxserver.v2.core.SFSEventParam;
import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.entities.data.SFSObject;
import com.smartfoxserver.v2.entities.variables.UserVariable;

public class UserVariablesHandler extends BaseServerEventHandler {
	@Override
	public void handleServerEvent(ISFSEvent event) throws SFSException{
		@SuppressWarnings("unchecked")
        List<UserVariable> variables = (List<UserVariable>) event.getParameter(SFSEventParam.VARIABLES);
		User user = (User) event.getParameter(SFSEventParam.USER);
		
		ISFSMMOApi mmoAPi = SmartFoxServer.getInstance().getAPIManager().getMMOApi();
		
		// Make a map of the variables list
		Map<String, UserVariable> varMap = new HashMap<String, UserVariable>();
		for (UserVariable var : variables)
		{
			varMap.put(var.getName(), var);
		}
		
		if (varMap.containsKey(ConstantClass.X) && varMap.containsKey(ConstantClass.Z))
		{
			double xpos = varMap.get(ConstantClass.X).getDoubleValue();
			double zpos = varMap.get(ConstantClass.Z).getDoubleValue();
						
			// Check Map X limits
			if (xpos < -100 || xpos > 100)
			{
				//Ext.LogMessage(ExtensionLogLevel.ERROR, String.format("NPCRunner : %s x is out of limits", user.getName()));
				ISFSObject sendObj = new SFSObject();
				sendObj.putUtfString(ConstantClass.ERROR_CODE, ErrorClass.SERVER_UNKNOWN_ERROR);
				sendObj.putUtfStringArray(ConstantClass.ERROR_MESSAGE, ErrorClass.GetErrorMessage(ErrorClass.SERVER_UNKNOWN_ERROR));
				this.send(ConstantClass.ERROR, sendObj, user);
				return;
			}
			
			// Check Map Z limits
			if (zpos < -100 || zpos > 100)
			{
				//Ext.LogMessage(ExtensionLogLevel.ERROR, String.format("NPCRunner : %s z is out of limits", user.getName()));
				ISFSObject sendObj = new SFSObject();
				sendObj.putUtfString(ConstantClass.ERROR_CODE, ErrorClass.SERVER_UNKNOWN_ERROR);
				sendObj.putUtfStringArray(ConstantClass.ERROR_MESSAGE, ErrorClass.GetErrorMessage(ErrorClass.SERVER_UNKNOWN_ERROR));
				this.send(ConstantClass.ERROR, sendObj, user);
				return;
			}
			
			Vec3D pos = new Vec3D
			(
				varMap.get(ConstantClass.X).getDoubleValue().floatValue(),
				1.0f,
				varMap.get(ConstantClass.Z).getDoubleValue().floatValue()
			);
			
			mmoAPi.setUserPosition(user, pos, this.getParentExtension().getParentRoom());
		}
	}
}
