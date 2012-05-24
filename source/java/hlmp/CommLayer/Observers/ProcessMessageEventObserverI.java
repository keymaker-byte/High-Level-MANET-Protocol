package hlmp.CommLayer.Observers;

import hlmp.CommLayer.Messages.Message;

/**
 * Observer para eventos que reciben un Message
 * @author nicolas
 *
 */
public interface ProcessMessageEventObserverI {
	public void processMessageUpdate(Message m);
}
