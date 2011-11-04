package hlmp.NetLayer;

import hlmp.Tools.BitConverter;

import java.io.IOException;
import java.io.InputStream;

public class ListenTCPMessagesThread extends Thread {

	RemoteMachine remoteMachine;
	NetHandler netHandler;
	
	public ListenTCPMessagesThread(NetHandler netHandler) {
		super();
		this.netHandler = netHandler;
	}

	public void setRemoteMachine(RemoteMachine remoteMachine) {
		this.remoteMachine = remoteMachine;
	}

	@Override
	public void run() {
		try
        {
			
            while (true)
            {
                InputStream nStream = remoteMachine.getTcpClient().getInputStream();
                byte[] length = new byte[4];
                int m = nStream.read(length, 0, 4);
                while (m < 4)
                {
                    m += nStream.read(length, m, 4 - m);
                }
                byte[] data = new byte[BitConverter.byteArrayToInt(length)];
                int n = nStream.read(data, 0, data.length);
                while (n < data.length)
                {
                    n += nStream.read(data, n, data.length - n);
                }
                NetMessage message = new NetMessage(data);
                netHandler.addTCPMessages(message);
            }
        }
//        catch (InterruptedException e)
//        {
//            throw e;
//        }
//        catch (IOException e) {
//			// TODO Auto-generated catch block
//			e.printStackTrace();
//		}
        catch (Exception e)
        {
            netHandler.informationNetworkingHandler("TCP WARNING: header reading failed " + e.getMessage());
            netHandler.disconnectFrom(remoteMachine);
        }
	}

	
}
