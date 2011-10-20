package hlmp.NetLayer;

/**
 * Clase que representa un dispositivo de red del sistema
 */
public class NetworkAdapter {
	
	/**
	 * El Index del dispositivo, en la lista de dispositivos del sistema operativo
	 */
	private int index;
	
	/**
	 * El SettingID del dispositivo en la lista de dispositivos del sistema operativo
	 */
	//private String id;
	
	/**
	 * El Description del dispositivo en la lista de dispositivos del sistema operativo
	 */
	private String description;
	
	/**
	 * Constructor vacío
	 */
	public NetworkAdapter(){
		
	}
	
	@Override
	/**
	 * Metodo toString sobreescrito
	 * @return la representación en String
	 */
	public String toString() {
		if( this.description.length() > 30 ){
			return this.description.substring(0, 30);
		}else if( this.description.length() <= 1 ){
			return "device";
		}
		return this.description;
	}

	@Override
	/**
	 * Sobreescribe equals
	 * @param o otro objeto
	 * @return true si tienen el mismo Index, false si no
	 */
	public boolean equals(Object o) {
		
		if( o != null && o.getClass().equals(this.getClass())){
			NetworkAdapter aobj = (NetworkAdapter) o;
			if( this.index == aobj.index){
				return true;
			}
		}
		return false;
	}	
	
	@Override
	/**
	 * HasChode
	 * @return el hashcode del padre
	 */
	public int hashCode() {
		return super.hashCode();
	}
}