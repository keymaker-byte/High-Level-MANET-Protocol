package hlmp.NetLayer;

import hlmp.Tools.BitConverter;

import java.io.IOException;
import java.io.OutputStream;
import java.net.InetAddress;
import java.net.Socket;
import java.util.UUID;
import java.util.concurrent.atomic.AtomicInteger;

public class RemoteMachine{

	/**
	 * La Ip de la máquina remota
	 */
	private InetAddress ip;
	/**
	 * El cliente TCP con el cual se puede enviar/recibir mensajes de la maquina remota
	 */
	private Socket tcpClient;
	/**
	 * El thread que escucha los mensajes TCP provenientes de la maquina remota
	 */
	private ListenTCPMessagesThread clientThread;
	/**
	 * El ID unico de esta maquina remota
	 */
	private String id;
	/**
	 * Cantidad de fallas seguidas que se han originado al intentar enviar un mensaje
	 */
	private int fails;
	
	
	/**
	 * Lock para envio de mensajes TCP
	 */
    private Object sendTCPLock;
	private AtomicInteger closePoint;
    private OutputStream senderStream;
	
	/**
	 * Constructor parametrizado
	 * @param ip la ip de la maquina remota
	 * @param tcpClient el objeto TcpClient asociado
	 * @param clientThread el thread que maneja lectura de datos de la maquina
	 */
	public RemoteMachine(InetAddress ip, Socket tcpClient,	ListenTCPMessagesThread clientThread) {
		this.ip = ip;
		this.tcpClient = tcpClient;
		this.clientThread = clientThread;
		this.id = UUID.randomUUID().toString();
		this.sendTCPLock = new Object();
		this.fails = 0;
		this.closePoint = new AtomicInteger(0);
	}

	/**
	 * Envia un mensaje de red a la maquina remota
	 * @param netMessage El mensaje de red
	 * @param timeOutWriteTCP Tiempo de espera para enviar el mensaje
	 * @throws IOException 
	 */
	public void sendNetMessage(NetMessage netMessage, int timeOutWriteTCP) throws IOException{
		synchronized(sendTCPLock)
		{
			fails++;
			senderStream = tcpClient.getOutputStream(); 
			try
			{
				tcpClient.setSoLinger(timeOutWriteTCP!=0, timeOutWriteTCP);
				
				byte[] lenght = BitConverter.intToByteArray(netMessage.getSize());
				//byte[] netByteMessage = new byte[4 + netMessage.getSize()];
				senderStream.write(lenght, 0, 4);
				senderStream.write(netMessage.getBody(), 0, netMessage.getSize());
				// NM: lo cambie para que no se cierre y pueda enviar varios mensajes seguidos.
				//senderStream.close();
				fails = 0;
			}
//            catch (InterruptedException e)
//            {
//                try
//                {
//                    senderStream.close();
//                }
//                catch (Exception e2)
//                {
//                } 
//                throw e;
//            }
			catch (IOException e) {
				try
				{
					senderStream.close();
				}
				catch (Exception e2)
				{
				}
				throw e;
			}
		} 
	}
	
	/**
	 * Cierra la conexion a esta maquina remota.. los thread abort exception no detienen la ejecución 
	 * @throws InterruptedException 
	 * @throws IOException 
	 */
	public void close() throws InterruptedException, IOException{

		if( closePoint.compareAndSet(0, 1))
        {
            try
            {
                senderStream.close();
            }
            catch (IOException e)
            {
                throw e;
            }
//            catch (InterruptedException e)
//            {
//                throw e;
//            }
            catch (Exception e)
            {
            }
            try
            {
                OutputStream nStream = tcpClient.getOutputStream();
                nStream.close();
            }
//            catch (InterruptedException e)
//            {
//                throw e;
//            }
            catch (IOException e)
            {
                throw e;
            }
            catch (Exception e)
            {
            }
            try
            {
            	// TODO: FVALVERD creo que da igual este parametro
            	tcpClient.setSoLinger(false, 0);
            	// TODO: FVALVERD parametrizar el 1000
            	tcpClient.setSoTimeout(1000);
                tcpClient.close();
            }
//            catch (InterruptedException e)
//            {
//                throw e;
//            }
            catch (IOException e)
            {
                throw e;
            }
            catch (Exception e)
            {
            }
            try
            {
                clientThread.interrupt();
                clientThread.join();
            }
            catch (InterruptedException e)
            {
                throw e;
            }
            catch (Exception e)
            {
            }
            closePoint.set(0); 
        }
	}

	public InetAddress getIp() {
		return ip;
	}

	public String getId() {
		return id;
	}

	public int getFails() {
		return fails;
	}

	public Socket getTcpClient() {
		return tcpClient;
	}

	
}
