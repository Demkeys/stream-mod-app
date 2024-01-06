# This script generates the OAuth request URL for the Twitch API.
from email.policy import default
import urllib.parse
import sys
import socket

client_id = ''
scope = 'moderator:read:chatters moderator:read:followers'
remote_addr = ('192.168.8.111', 9010)

def generate_auth_url():
    global client_id
    redirect_uri = 'https://localhost:3000'
    global scope

    url = 'https://id.twitch.tv/oauth2/authorize?'
    query_dict = {
        'client_id':client_id,
        'redirect_uri':redirect_uri,
        'response_type':'token',
        'scope':scope
    }
    query_str = urllib.parse.urlencode(query_dict)

    print(url+query_str)

def send_auth_data():
    global client_id
    s = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    authPair = f'{client_id}|{sys.argv[2]}'
    buff = bytes(authPair, 'ascii')
    global remote_addr
    s.sendto(buff, remote_addr)
    s.close()

def main():
    if __name__ == "__main__":
        if client_id == '':
            print('No ClientID specified.')
            return
        if len(sys.argv) < 1:
            print('Error')
            print('Usage: Pass in argument "generate" or "send".')
        else:
            match sys.argv[1]:
                case 'generate':
                    generate_auth_url()
                case 'send':
                    if len(sys.argv) < 3:
                        print('Missing argument.')
                    else:
                        send_auth_data()
                case _:
                    print('Incorrect argument.')

main()