{
	"WebAPIConfiguration": {

		// Host is the IP address of the local NIC to listen for requests to the web API.
		"Host": "0.0.0.0",

		/* 
		 *  The port to listen for requests to the web API.
		 *  If the main ACE server Port setting is 9000 then 9000-9001 are already occupied.  The main ACE server Port + 2 is recommended.
		 */
		"Port": 9002,

		/*
		 *  ExternalIPAddressOrDNSName is used to form internet accessible URLs, example: superserver.socloudyinthere.com
		 *  Used to conduct interserver communications.
		 */
		"ExternalIPAddressOrDNSName": "",

		/*
		 *  ExternalPort is used to form internet accessible URLs, example: 9002
		 *  Used to conduct interserver communications.
		 *  This is normally set to the same value as Port
		 */
		"ExternalPort": 9002

	}
}