package hlmp.commlayer.observers;

import hlmp.commlayer.messages.Message;

/**
 * Observer para eventos que reciben un Message
 * @author nicolas
 *
 */
public interface MessageEventObserverI {
	public void update(Message m);
}
