package hlmp.CommLayer.Observers;

import hlmp.CommLayer.NetUser;

/**
 * Observer para eventos que reciben un NetUser
 * @author nicolas
 *
 */
public interface NetUserEventObserverI {

	public void update(NetUser user);
}
