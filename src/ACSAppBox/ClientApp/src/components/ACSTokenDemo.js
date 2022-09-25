import React, { Component } from 'react';
import authService from './api-authorization/AuthorizeService'

export class ACSTokenDemo extends Component {
  static displayName = ACSTokenDemo.name;

  constructor(props) {
    super(props);
    this.state = { acsToken: undefined, loading: true };
  }

  componentDidMount() {
    this.fetchACSToken();
  }

  static renderToken(acsToken) {
    return (
      <div style={{border: '1px solid red'}}>
        {acsToken.token}
      </div>
    );
  }

  render() {
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
      : ACSTokenDemo.renderToken(this.state.acsToken);

    return (
      <div>
        <h1 id="tabelLabel" >ACS Token</h1>
        <p>This component demonstrates fetching ACS token from server.</p>
        {contents}
      </div>
    );
  }

  async fetchACSToken() {
    const token = await authService.getAccessToken();
    const response = await fetch('/api/communicationuser/token', {
      headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
    });
    const data = await response.json();
    this.setState({ acsToken: data, loading: false });
  }
}
