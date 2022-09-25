import React, { Component } from 'react';
import authService from './api-authorization/AuthorizeService'

export class Rooms extends Component {
  static displayName = Rooms.name;

  constructor(props) {
    super(props);
    this.state = { 
      rooms: [], 
      loading: true,
      isAuthenticated: false,
      userName: ''  
    };
    this.createRoom = this.createRoom.bind(this);
    this.deleteRoom = this.deleteRoom.bind(this);
  }

  async componentDidMount() {
    this.populateRoomsData();
    const [isAuthenticated, user] = await Promise.all([authService.isAuthenticated(), authService.getUser()])
    this.setState({
      ['isAuthenticated']: isAuthenticated,
      ['userName']: user && user.name
    });
  }

  render() {
    console.log(this.state.rooms)

    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
      : Rooms.renderRoomTable(this.state.rooms, this.deleteRoom, this.state.userName);

    return (
      <div>
        <button className="btn btn-primary" onClick={this.createRoom}>Create Room</button>
        {contents}
      </div>
    );
  }

  async createRoom() {
    const token = await authService.getAccessToken();
    const response = await fetch('api/Rooms', {method: "POST",
      headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
    })
    const data = await response.json();
    this.state.rooms.push(data)

    this.setState({
      rooms: this.state.rooms, loading: false
    });

  }

  async deleteRoom(index) {
    const roomToDelete = this.state.rooms[index];
    const token = await authService.getAccessToken();
    const response = await fetch('api/Rooms/' + roomToDelete.id, {method: "DELETE",
      headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
    })

    var roomsLeft = this.state.rooms.filter(function(ele){
      return ele != roomToDelete;
    });

    this.setState({
      rooms: roomsLeft, loading: false
    });

  }

  async populateRoomsData(){
    const token = await authService.getAccessToken();
    const response = await fetch('api/Rooms', {
      headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
    });
    const data = await response.json();
    this.setState({ rooms: data, loading: false });
  }

  static renderRoomTable(rooms, deleteRoom, userName) {
    console.log("render" + rooms)
    return (
      <table className='table table-striped' aria-labelledby="tabelLabel">
        <thead>
          <tr>
            <th>Id</th>
            <th>Join</th>
            <th>Delete</th>
          </tr>
        </thead>
        <tbody>
          {rooms.map((room, index) =>
            <tr key={room.id}>
              <td>{room.id}</td>
              <td><button className="btn btn-primary" onClick={() => window.location = '/join?roomId=' + room.id + '&displayName=' + userName}>Join</button></td>
              <td><button className="btn btn-primary" onClick={() => deleteRoom(index)}>Delete</button></td>
            </tr>
          )}
        </tbody>
      </table>
    );
  }
}



