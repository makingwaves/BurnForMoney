import React, { Component } from 'react';

import './TotalNumbers.css';

class TotalNumbers extends Component {
  render() {
    return (
      <div className="TotalNumbers">
        <div className="TotalNumebrs__container container">

          <h2 className="TotalNumbers__header Header"><strong className="flames">Burn For Money</strong> is a CSR internal initiative that connects charity with fit lifestyle.</h2>
          <h4>Our achievements so far:</h4>
          <div className="TotalNumbers__equation">
            <div className="TotalNumbers__equation-circle">
              <div>
                <span className="TotalNumbers__equation-value">{(this.props.data ? this.props.data.distance : 0)} km</span><br/>
                On route
              </div>
            </div>
            <div className="TotalNumbers__equation-operator">
              +
            </div>
            <div className="TotalNumbers__equation-circle">
              <div>
                <span className="TotalNumbers__equation-value">{(this.props.data ? this.props.data.time : 0)} h</span><br/>
                Of training
              </div>
            </div>
            <div className="TotalNumbers__equation-operator">
              =
            </div>
            <div className="TotalNumbers__equation-circle">
              <div>
                <span className="TotalNumbers__equation-value">{(this.props.data ? this.props.data.money : 0)} PLN</span><br/>
                Given to help
              </div>
            </div>
          </div>
        </div>
      </div>
    );
  }
}

export default TotalNumbers;
