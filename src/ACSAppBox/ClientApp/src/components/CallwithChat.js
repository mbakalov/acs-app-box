import React, { useEffect, useState } from 'react';
import {
    CallWithChatComposite,
    useAzureCommunicationCallWithChatAdapter,
    createAzureCommunicationCallWithChatAdapter
  } from '@azure/communication-react';
import { AzureCommunicationTokenCredential, CommunicationUserIdentifier } from '@azure/communication-common';

function CallWithChat(props){
    console.log("render call with chat");
    const [adapter , setAdapter] = useState();
    var userId = props.state.userId;
    var displayName = props.state.displayName;
    var callLocator = {groupId: props.state.groupId};
    var chatThreadId = props.state.threadId;
    var credential = new AzureCommunicationTokenCredential(props.state.accessToken);
    // const credential = useMemo(() => {
    //     try {
    //         new AzureCommunicationTokenCredential(props.state.accessToken)
    //     } catch {
    //         console.error('Failed to construct token credential');
    //         return undefined;
    //     }
    // }, [props.state.accessToken]);

    var endpoint = props.state.endpoint;

    useEffect(() => {
        const createAdapter = async () => {
            setAdapter(await createAzureCommunicationCallWithChatAdapter({
                userId, 
                displayName, 
                credential, 
                endpoint, 
                locator: {
                    callLocator,
                    chatThreadId
                }
            }))
        }
        createAdapter();
    }, [])

    // const adapter = useAzureCommunicationCallWithChatAdapter({
    //     userId,
    //     displayName,
    //     credential,
    //     endpoint,
    //     locator: {
    //         callLocator,
    //         chatThreadId
    //     }
    // });

    if (!adapter) {
        return <div>Initializing</div>
    }
    return <div style={{height: '70vh', width: '70vw'}}>
        <CallWithChatComposite adapter={adapter}/>
    </div>;
}

export default CallWithChat;
