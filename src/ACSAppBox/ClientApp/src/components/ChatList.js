import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import authService from './api-authorization/AuthorizeService'
import { AzureCommunicationTokenCredential } from '@azure/communication-common';
import { ChatClient } from '@azure/communication-chat';

const ChatList = (props) => {

  const [loading, setLoading] = useState(true);
  const [chatClient, setChatClient] = useState(null);
  const [chatThreads, setChatThreads] = useState(null);

  useEffect(() => {
    console.log('running effect');
    const loadChatThreads = async () => {
      console.log('called loadChatThreads');

      const token = await authService.getAccessToken();

      // get ACS token
      const acsTokenResponse = await fetch('/api/communicationuser/token', {
        headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
      });
      const acsToken = await acsTokenResponse.json();

      // get ACS endpoint
      const acsConfigResponse = await fetch('/api/appConfiguration');
      const acsConfig = await acsConfigResponse.json();

      const tokenCredential = new AzureCommunicationTokenCredential(acsToken.token);
      const cc = new ChatClient(
        acsConfig.communicationServicesEndpoint,
        tokenCredential);

      const threads = cc.listChatThreads();
      let loadedThreads = [];

      for await (const thread of threads) {
        loadedThreads.push(thread);
      }

      setChatClient(cc);
      setChatThreads(loadedThreads);
      setLoading(false);
    }

    loadChatThreads();
  }, []);
    
  return (
    <>
      { loading 
        ? <p><em>Loading...</em></p>
        : <table className='table table-striped' aria-labelledby="tabelLabel">
            <thead>
              <tr>
                <th>Id</th>
                <th>Topic</th>
              </tr>
            </thead>
            <tbody>
              {chatThreads.map(ct =>
                <tr key={ct.id}>
                  <td>
                    <Link to={`/chats/${ct.id}`}>{ct.id}</Link>
                  </td>
                  <td>{ct.topic}</td>
                </tr>
              )}
            </tbody>
          </table>
      }
    </>
  );
}

export default ChatList;