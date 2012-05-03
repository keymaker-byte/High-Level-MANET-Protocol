package hlmp.Tools;

import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.nio.charset.Charset;
import java.util.UUID;

public class BitConverter {

	public static byte[] intToByteArray(int value) {
	    ByteBuffer buffer = ByteBuffer.allocate(4).order(ByteOrder.LITTLE_ENDIAN);;
	    buffer.putInt(value);
	    return buffer.array();
	}
	
	public static final int byteArrayToInt(byte [] b) {
		ByteBuffer bb = ByteBuffer.wrap(b).order(ByteOrder.LITTLE_ENDIAN);
		return bb.getInt();
	}
	
	public static byte[] UUIDtoBytes(UUID id) {
		byte[] bits = new byte[16];
		long msb = id.getMostSignificantBits();
		long lsb = id.getLeastSignificantBits();
		
		for (int i = 0; i < 8; i++) {
			bits[i] = (byte) (msb >>> 8 * i);
		}
		for (int i = 8; i < 16; i++) {
			bits[i] = (byte) (lsb >>> 8 * (i - 16));
		}
		
		return bits;
	}
	
	public static UUID bytesToUUID(byte[] bits) {
		return new UUID(readLong(bits, 0), readLong(bits, 8));
	}

	public static final int readInt(byte[] src, int offset) {
		ByteBuffer bb = ByteBuffer.wrap(src, offset, 4).order(ByteOrder.LITTLE_ENDIAN);
		return bb.getInt();
 	}
	
	public static void writeInt(int datum, byte[] dst, int offset) {
		byte[] datumToByteArray = intToByteArray(datum);
		for (int i=0; i<4; ++i){
			dst[offset+i] = datumToByteArray[i]; 
		}
	}
	
	public static void writeLong(long datum, byte[] dst, int offset) {
		ByteBuffer buffer = ByteBuffer.allocate(8).order(ByteOrder.LITTLE_ENDIAN);
	    buffer.putLong(datum);
	    byte[] datumToByteArray = buffer.array();
	    
	    for (int i=0; i<8; ++i) {
			dst[offset+i] = datumToByteArray[i]; 
		}
 	}

	public static final long readLong(byte[] src, int offset) {
		ByteBuffer bb = ByteBuffer.wrap(src, offset, 8).order(ByteOrder.LITTLE_ENDIAN);
		return bb.getLong();
 	}
	
	public static final byte[] stringToByte(String s) {
		return s.getBytes(Charset.forName("UTF-16LE"));
	}
	
	public static final String byteToString(byte[] b) {
		return new String(b, Charset.forName("UTF-16LE"));
	}
}
