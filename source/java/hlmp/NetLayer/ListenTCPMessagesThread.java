package hlmp.NetLayer;

import hlmp.Tools.BitConverter;

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
			InputStream nStream = remoteMachine.getTcpClient().getInputStream();
            while (true)
            {
                byte[] sizeLikeByte = new byte[4];
                int totalReadBytes = nStream.read(sizeLikeByte, 0, 4);
                if (totalReadBytes == -1) this.interrupt();
                while (totalReadBytes < 4)
                {
                	int currentReadBytes = nStream.read(sizeLikeByte, totalReadBytes, 4 - totalReadBytes); 
                	if (currentReadBytes == -1) this.interrupt();
                    totalReadBytes += currentReadBytes;
                }
                int size = BitConverter.byteArrayToInt(sizeLikeByte);
                byte[] data = new byte[size];
                totalReadBytes = nStream.read(data, 0, data.length);
                while (totalReadBytes < data.length)
                {
                	int currentReadBytes = nStream.read(sizeLikeByte, totalReadBytes, 4 - totalReadBytes); 
                	if (currentReadBytes == -1) this.interrupt();
                    totalReadBytes += currentReadBytes;
                }
                NetMessage message = new NetMessage(data);
                netHandler.addTCPMessages(message);
            }
        }
		catch (Exception e)
        {
        	if (!this.isInterrupted())
        	{
        		netHandler.informationNetworkingHandler("TCP WARNING: header reading failed " + e.getMessage());
        	}
        	netHandler.disconnectFrom(remoteMachine);
        }
	}

	
}
