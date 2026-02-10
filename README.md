The current version is theoretically compatible with any battery-powered Windows system (Emulators are excluded, as they could send false data, resulting in a constant red screen).

Due to the way PDMS is structured, it (logically) ignores Windows actions, meaning it intervenes too quickly, which can lead to data loss.

A precursor, possibly "last_secure.val," should be added to tell all apps (created via MaidNVI/ADK) to create a cache. Currently, a cache system is missing nor being designed.

Please start any application (including the background worker) as administrator to prevent any problems in this regard. The tests show that everything would work even without admin access, however I'm not so sure how well that might turn out (especially with Windows S mode).

However, the system is not designed for every scenario. Incorrect setup can render the device unbootable and potentially brick it. In order to be able to restore everything in such a disaster scenario, I am creating a Firmware Restore USB, which should act as an update for XELA, update for Core Dictionary (the most important aspect for our scenario here), update for MELA or as a restore USB.

Please use it with caution. I am not responsible for data loss or users who are locked out of the system (but I could try my best to support them).
