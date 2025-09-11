package weaving.playasone;

import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.entities.data.SFSObject;
import com.smartfoxserver.v2.extensions.BaseClientRequestHandler;

public class PingPongHandler extends BaseClientRequestHandler {

	@Override
	public void handleClientRequest(User sender, ISFSObject params)
	{
		((PlayAsOneExtension) this.getParentExtension()).LogMessage("KEEPALIVE");
		
		ISFSObject sendObj = new SFSObject();
		this.send(ConstantClass.KEEPALIVE, sendObj, sender);
	}
}
