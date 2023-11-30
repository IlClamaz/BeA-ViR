using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class UDPSender : MonoBehaviour
{
	[SerializeField] private string ip;
	[SerializeField] private int door;
	[SerializeField, TextArea(5,10)] private string data;

	private UdpClient c;
	private IPEndPoint ep;

	void Start()
	{
		c = new UdpClient();
	}

	[Button]
	public void SendData()
    {
		SendData(ip, door, data);
	}

	public void SendData(string ip_text, int port, string stringData)
	{
		IPAddress ip = IPAddress.Parse(ip_text);

		ep = new IPEndPoint(ip, port);
		c.Connect(ep);

		byte[] data = Encoding.ASCII.GetBytes(stringData);
		c.Send(data, data.Length);

		Debug.Log($"Sending data to [{ip}:{port}] [{data.Length} Bytes]");

		//Debug.Log($"Sending data to {GetBrodcastAddress(ip, }");
	}


	public static IPAddress GetBrodcastAddress(IPAddress address, IPAddress mask)
	{
		var addressInt = BitConverter.ToInt32(address.GetAddressBytes(), 0);
		var maskInt = BitConverter.ToInt32(mask.GetAddressBytes(), 0);
		var broadcastInt = addressInt | ~maskInt;
		return new IPAddress(BitConverter.GetBytes(broadcastInt));
	}

	[Button]
	public static IPAddress GetBroadcastAddress(string ipAddress, string subnetMask)
	{
		IPAddress ip = IPAddress.Parse(ipAddress);
		IPAddress sm = IPAddress.Parse(subnetMask);

		var addressInt = BitConverter.ToInt32(ip.GetAddressBytes(), 0);
		var maskInt = BitConverter.ToInt32(sm.GetAddressBytes(), 0);
		var broadcastInt = addressInt | ~maskInt;

		IPAddress broadcastAddress = new IPAddress(BitConverter.GetBytes(broadcastInt));
		Debug.Log($"Broadcast: {broadcastAddress}");
		return broadcastAddress;
	}

	[Button]
	public static void EnumerateNetworkDevices()
	{
		foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
		{
			Debug.Log($"TYPE: {adapter.NetworkInterfaceType}");

			foreach (UnicastIPAddressInformation unicastIPAddressInformation in adapter.GetIPProperties().UnicastAddresses)
			{
				if (unicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
				{
					Debug.Log($" Device: {adapter.Name} IP: {unicastIPAddressInformation.Address} MASK: {unicastIPAddressInformation.IPv4Mask}");
				}
			}
		}
	}
}
