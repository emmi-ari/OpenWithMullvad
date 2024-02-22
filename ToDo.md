# Requirements
* Mullvad API
	* Get up to date server list
* Mullvad CLI
	* `mullvad status` -> Connected/Disconnected status
	* `mullvad connect`
* "Something went wrong with the VPN connection (validation)" handler
	* Retry
	* Don't start application (cancel)
	* Start application anyway (flag for no VPN connection/unvalidated connection)
	* Start Mullvad GUI