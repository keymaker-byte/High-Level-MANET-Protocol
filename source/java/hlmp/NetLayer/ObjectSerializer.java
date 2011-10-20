package hlmp.NetLayer;

import java.io.ByteArrayInputStream;
import java.io.StringWriter;
import javax.xml.bind.JAXBContext;
import javax.xml.bind.JAXBException;
import javax.xml.bind.Marshaller;
import javax.xml.bind.Unmarshaller;

public class ObjectSerializer {
	
	private JAXBContext context;
	private Marshaller m;
	private Unmarshaller um;

	public ObjectSerializer(Object tipo) {
		try {
			context = JAXBContext.newInstance(tipo.getClass());
			m = context.createMarshaller();
			um = context.createUnmarshaller();
			//m.setProperty(Marshaller.JAXB_FORMATTED_OUTPUT, true);
		} catch (JAXBException e) {
			e.printStackTrace();
		}
		
	}
	
	public String serialize(Object obj){
		StringWriter sw = new StringWriter();
		String s="";
		try {
			m.marshal(obj, sw);
			s = sw.toString();
		} catch (JAXBException e) {
			e.printStackTrace();
		}
		return s;
	}
	
	public Object unserialize(String obj){
		ByteArrayInputStream bs = new ByteArrayInputStream(obj.getBytes());
		Object o = null;
		try {
			o = (Object) um.unmarshal(bs);
		} catch (JAXBException e) {
			e.printStackTrace();
		}
		return o;
	}

}
