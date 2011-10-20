package hlmp.CommLayer.Messages;

import java.nio.charset.Charset;
import java.util.UUID;

import hlmp.CommLayer.Constants.*;
import hlmp.Tools.BitConverter;

public class ImAliveMessage extends MulticastMessage {

	/**
	 * Default Constructor
	 */
    public ImAliveMessage(){
    	super();
        this.type = MessageType.IMALIVE;
        this.protocolType = MessageProtocolType.HLMP;
    }
	
	@Override
	public byte[] makePack() {
		
        byte[] userName = this.senderNetUser.getName().getBytes(Charset.forName("UTF-8"));//userNameSize (4 - userNameSize + 3)
        byte[] userNameSize = BitConverter.intToByteArray(userName.length); //4 (0 - 3)
        
        byte[] userNeighborhoodSize = BitConverter.intToByteArray(this.senderNetUser.getNeighborhoodIds().length);//4 (userNameSize + 4 - userNameSize + 7)
        byte[] userNeighborhood = new byte[this.senderNetUser.getNeighborhoodIds().length * 16];//userNeighborhoodSize*16 (userNameSize + 8 - userNameSize + 7 + userNeighborhoodSize*16)
        for (int i = 0; i < this.senderNetUser.getNeighborhoodIds().length; i++)
        {
            byte[] neighborId = BitConverter.UUIDtoBytes(this.senderNetUser.getNeighborhoodIds()[i]);
            System.arraycopy(neighborId, 0, userNeighborhood, i * 16, 16);
        }
        byte[] userState = BitConverter.intToByteArray(this.senderNetUser.getState());//4 (userNameSize + 8 + userNeighborhoodSize*16  --  userNameSize + 11 + userNeighborhoodSize*16)
        //upLayerDataSize (userNameSize + 12 + userNeighborhoodSize*16  --  userNameSize + 11 + userNeighborhoodSize*16 + upLayerDataSize)

        byte[] pack = new byte[userName.length + 12 + this.senderNetUser.getNeighborhoodIds().length * 16 + this.senderNetUser.getUpLayerData().length];
        System.arraycopy(userNameSize, 0, pack, 0, 4);
        System.arraycopy(userName, 0, pack, 4, userName.length);
        System.arraycopy(userNeighborhoodSize, 0, pack, userName.length + 4, 4);
        System.arraycopy(userNeighborhood, 0, pack, userName.length + 8, this.senderNetUser.getNeighborhoodIds().length * 16);
        System.arraycopy(userState, 0, pack, userName.length + 8 + this.senderNetUser.getNeighborhoodIds().length * 16, 4);
        System.arraycopy(this.senderNetUser.getUpLayerData(), 0, pack, userName.length + 12 + this.senderNetUser.getNeighborhoodIds().length * 16, this.senderNetUser.getUpLayerData().length);

        return pack;
	}

	@Override
	public void unPack(byte[] messagePack) {
		int userNameSize = BitConverter.readInt(messagePack, 0);
        this.senderNetUser.setName(new String(messagePack, 4, userNameSize, Charset.forName("UTF-8")));
        int userNeighborhoodSize = BitConverter.readInt(messagePack, userNameSize + 4);
        this.senderNetUser.setNeighborhoodIds(new UUID[userNeighborhoodSize]);
        for (int i = 0; i < userNeighborhoodSize; i++)
        {
            byte[] neighborId = new byte[16];
            System.arraycopy(messagePack, userNameSize + 8 + i * 16, neighborId, 0, 16);
            this.senderNetUser.setNeighborhoodId(i, BitConverter.bytesToUUID(neighborId));
        }
        this.senderNetUser.setState(BitConverter.readInt(messagePack, userNameSize + 8 + userNeighborhoodSize * 16));
        int upLayerDataSize = messagePack.length - (userNameSize + 12 + userNeighborhoodSize * 16);
        if (upLayerDataSize > 0)
        {
            byte[] temp = new byte[upLayerDataSize];
            System.arraycopy(messagePack, userNameSize + 12 + userNeighborhoodSize * 16, temp, 0, upLayerDataSize);
            this.senderNetUser.setUpLayerData(temp);
        }

	}
	/**
	 * Sobreescribe el metodo toString
	 * @return El string que representa este objeto
	 */
	@Override
    public String toString()
    {
        return super.toString() + "ImAliveMessage";
    }

}
