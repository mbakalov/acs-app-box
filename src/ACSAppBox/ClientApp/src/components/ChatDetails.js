import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import authService from './api-authorization/AuthorizeService';
import { AzureCommunicationTokenCredential } from '@azure/communication-common';
import {
    ChatComposite,
    fromFlatCommunicationIdentifier,
    createAzureCommunicationChatAdapter
  } from '@azure/communication-react';

const ChatDetails = (props) => {

  const { threadId } = useParams();
  const [adapter, setAdapter] = useState(null);

  useEffect(() => {
    console.log('ChatDetails: running effect');

    const createChatAdapter = async () => {
      console.log('called createChatAdapter');

      const token = await authService.getAccessToken();
      const user = await authService.getUser();

      // get ACS token
      const acsTokenResponse = await fetch('/api/communicationuser/token', {
        headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
      });
      const acsToken = await acsTokenResponse.json();

      // get ACS endpoint
      const acsConfigResponse = await fetch('/api/appConfiguration');
      const acsConfig = await acsConfigResponse.json();


      const createdAdapter = await createAzureCommunicationChatAdapter({
        endpoint: acsConfig.communicationServicesEndpoint,
        credential: new AzureCommunicationTokenCredential(acsToken.token),
        displayName: user.name,
        threadId: threadId,
        userId: fromFlatCommunicationIdentifier(acsToken.userId)
      });

      setAdapter(createdAdapter);
    }

    createChatAdapter();
    
  }, [threadId]);

  if (adapter) {
    return (
      <div style={{ height: '90vh', width: '90vw' }}>
        <ChatComposite
          adapter={adapter}
          options={{
            topic: true
          }}
        />
      </div>
    );
  }

  return <>Initializing...</>
}

export default ChatDetails;