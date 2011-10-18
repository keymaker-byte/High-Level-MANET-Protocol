package hlmp.commlayer.observers;

import hlmp.commlayer.NetUser;

/**
 * Observer para eventos que reciben un NetUser
 * @author nicolas
 *
 */
public interface NetUserEventObserverI {

	public void update(NetUser user);
}
