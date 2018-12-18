import React, { Component } from 'react';
import { Link } from 'react-router-dom';

import './AthletesList.css';

class AthletesList extends Component {

  constructor(props) {
      super(props);
      this.state = { athletes: []} ;
    }

  render() {
    return (
      <div className="AthletesList">
        <ul>
          {this.state.athletes.map( (athlete)=>  <li key={athlete.id} ><Link to={`/dashboard/athlete/${athlete.id}`}>{athlete.firstName} {athlete.lastName}</Link></li>)}
        </ul>
      </div>
    );
  }
  componentDidMount(){
    // internal api_url
    const api_url = process.env.REACT_APP_DASHBOARD_API_URL;

    fetch(api_url+"api/athletes")
      .then(res => res.json())
      .then(
        (result) => {this.setState({athletes: result }); },
        (error) => {this.setState({athletes: null}); console.error('Error:', error); }
      );

  }

}
export default AthletesList;
