package hlmp.NetLayer;

import java.io.StringReader;
import java.io.StringWriter;

import org.simpleframework.xml.Serializer;
import org.simpleframework.xml.core.Persister;


/**
 * Modificado para Android 2.3 por felipe.valverde.campos@gmail.com
 *
 */

public class ObjectSerializer {
	
	public String serialize(Object obj){
		StringWriter sw = new StringWriter();
		String serialized_object = "";
		Serializer serial = new Persister();
		try {
			serial.write(obj, sw);
			serialized_object = sw.toString();
		} catch (Exception e) {
			e.printStackTrace();
		}
		return serialized_object ;
	}
	
	public Object unserialize(String serialized_object, Class<?> cls){
		Serializer serializer = new Persister();
		StringReader sr = new StringReader(serialized_object);
		Object object = null;
		try {
			object = serializer.read(cls, sr);
		} catch (Exception e) {
			e.printStackTrace();
		}
		return object;
	}
}