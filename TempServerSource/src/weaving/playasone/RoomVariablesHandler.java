package weaving.playasone;

import com.smartfoxserver.v2.core.ISFSEvent;
import com.smartfoxserver.v2.core.SFSEventParam;
import com.smartfoxserver.v2.entities.variables.RoomVariable;
import com.smartfoxserver.v2.exceptions.SFSException;
import com.smartfoxserver.v2.extensions.BaseServerEventHandler;

import java.util.List;

public class RoomVariablesHandler extends BaseServerEventHandler {
	@Override
	public void handleServerEvent(ISFSEvent event) throws SFSException{
		@SuppressWarnings("unchecked")
	    List<RoomVariable> changedVars = (List<RoomVariable>) event.getParameter(SFSEventParam.VARIABLES);

	    // 넘어온 변수들을 안전하게 순회하며 로그를 남깁니다.
	    for(RoomVariable variable : changedVars)
	    {    
	    	String key = variable.getName();
	    	Object value = variable.getValue();
	    	((PlayAsOneExtension) this.getParentExtension()).LogMessage(String.format("RoomVariable Updated -> Key: %s, Value: %s", key, value.toString()));
	    }
	}
}

