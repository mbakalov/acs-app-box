import React, { Component } from 'react';
import { Container } from 'reactstrap';
import { NavMenu } from './NavMenu';

export class Layout extends Component {
  static displayName = Layout.name;

  constructor(props) {
    super(props);
    this.state = {
      iFrame: 'false'
    };
  }

  componentDidMount(){
    let windowsURL = window.location.search;
    let params = new URLSearchParams(windowsURL);
    console.log(params.toString())
    let iFrame = params.get('iFrame');
    console.log(iFrame);
    if (!iFrame) iFrame = 'false';
    this.setState({
      ["iFrame"]: iFrame
    })
  }

  render() {
    return (
      <div>
        {(this.state.iFrame === 'false') && <NavMenu />}
        <Container>
          {this.props.children}
        </Container>
      </div>
    );
  }
}
