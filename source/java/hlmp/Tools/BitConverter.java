package hlmp.Tools;

import java.nio.charset.Charset;
import java.util.UUID;

public class BitConverter {

	public static final byte[] intToByteArray(int value) {
		return new byte[] {
				(byte)(value >>> 24),
				(byte)(value >>> 16),
				(byte)(value >>> 8),
				(byte)value};
	}

	public static final int byteArrayToInt(byte [] b) {
		return (b[0] << 24)
		+ ((b[1] & 0xFF) << 16)
		+ ((b[2] & 0xFF) << 8)
		+ (b[3] & 0xFF);
	}
	
	public static byte[] UUIDtoBytes(UUID id){
		//return id.toString().getBytes();
		byte[] bits = new byte[16];
		writeLong(id.getMostSignificantBits(), bits, 0);
		writeLong(id.getLeastSignificantBits(), bits, 8);
		return bits;
	}
	
	public static UUID bytesToUUID(byte[] bits){
		//return UUID.fromString(new String(bits));
		return new UUID(readLong(bits, 0), readLong(bits, 8));
	}

	
	public static void writeInt(int datum, byte[] dst, int offset) {
 		dst[offset] = (byte) (datum >> 24);
 		dst[offset+1] = (byte) (datum >> 16);
 		dst[offset+2] = (byte) (datum >> 8);
 		dst[offset+3] = (byte)  datum;
	}
	
	public static void writeLong(long datum, byte[] dst, int offset) {
 		writeInt((int) (datum >> 32), dst, offset);
 		writeInt((int) datum, dst, offset + 4);
 	}

  	

  	public static final int readInt(byte[] src, int offset) {
 		return (src[offset] << 24) | ((src[offset + 1] & 0xff) << 16) |
     		        ((src[offset + 2] & 0xff) << 8) | (src[offset + 3] & 0xff);
 	}  	

	public static final long readLong(byte[] src, int offset) {
 		return ((long) readInt(src,offset) << 32) | 
 			 ((long) readInt(src,offset + 4) & 0xffffffffL);
 	}
	
	public static final byte[] stringToByte(String s){
		return s.getBytes(Charset.forName("UTF-8"));
	}
	
	public static final String byteToString(byte[] b){
		return new String(b, Charset.forName("UTF-8"));
	}
}
