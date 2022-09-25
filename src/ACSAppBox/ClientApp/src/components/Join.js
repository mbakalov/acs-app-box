import React, { Component } from 'react';
import authService from './api-authorization/AuthorizeService'
import CallWithChat from './CallwithChat';

export class Join extends Component {
  constructor(props) {
    super(props);
    this.state = {
      displayName: '',
      userId: '',
      accessToken: '',
      roomId: '',
      threadId: '',
      groupId: '',
      context: false,
      iFrame: false,
      endpoint: ''
    };
    this.handleChange = this.handleChange.bind(this);
    this.handleSubmit = this.handleSubmit.bind(this);
  }

  async componentDidMount(){
    let windowsURL = window.location.search;
    let params = new URLSearchParams(windowsURL);
    let roomId = params.get('roomId');
    let displayName = params.get('displayName');
    let iFrame =  params.get('iFrame');
    if (!iFrame) iFrame = 'false';
    this.setState({iFrame: iFrame});

    if(!displayName) 
    {
      const isAuthenticated = await authService.isAuthenticated();
      if (isAuthenticated) {
        const user = await authService.getUser();
        displayName = user.name;
      } else {
        displayName = "Default Name";
      }
      
    }
    this.setState({displayName: displayName});

    if(roomId) {
      console.log(roomId)
      this.setState({
        roomId: roomId
      })
      await this.getRoom(roomId, displayName)
    }
  }

  async getRoom(roomId, displayName) {
    const isAuthenticated = await authService.isAuthenticated();

    if (isAuthenticated) {
      const user = await authService.getUser();
      const token = await authService.getAccessToken();

      const uri = encodeURI(`api/rooms/${roomId}/join`);
      const responseContext = await fetch(uri, {
        headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
      });
      const context = await responseContext.json();
      console.log(context)
      this.setState({
        threadId: context.room.threadId,
        groupId: context.room.groupId,
        userId: context.userId,
        displayName: user.name,
        accessToken: context.accessToken,
        context: true,
        endpoint: context.endpoint
      })
    } else {
      const uri = encodeURI(`api/rooms/${roomId}/anonymousJoin?displayName=${displayName}`);
      const responseContext = await fetch(uri);
      const context = await responseContext.json();
      console.log(context)
      this.setState({
        threadId: context.room.threadId,
        groupId: context.room.groupId,
        userId: context.userId,
        accessToken: context.accessToken,
        context: true,
        endpoint: context.endpoint
      })
    }
  }

  async handleSubmit(event){
    event.preventDefault();
    await this.getRoom(this.state.roomId, this.state.displayName);
  }

  handleChange(event) {
    this.setState({
      [event.target.name]: event.target.value
    });
  }

  render() {
    let contents = this.state.context
    ? <CallWithChat state={this.state} />
    : <p><em>Enter Information...</em></p>

    console.log(this.state)

    return (
      <div>
        {(this.state.iFrame === 'false') && Join.renderForm(this)}
        {contents}
      </div>
    );
  }

  static renderForm(state) {
    return (
      <div>
        <form onSubmit={state.handleSubmit}>
          <label>
            Display Name:
            <input type="text" name="displayName" onChange={state.handleChange} value={state.state.displayName}/>
          </label>
          <br/>
          <br/>
          <label>
            Room Id:  
            <input type="text" name="roomId" onChange={state.handleChange} value={state.state.roomId}/>
          </label>
          <br/>
          <br/>
          <input type="submit" value="Join"/>
        </form>
        <br/>
        <br/>
        IFrame: 
        <br/>
        {`<iframe src='${window.location.origin}/join?iFrame=true&roomId=` + state.state.roomId + "&displayName=" + state.state.displayName + "' height='1000px' width=1000px' allow='camera;microphone'/>"}
        <br/>
        <br/>
      </div>
    );
  }
}
