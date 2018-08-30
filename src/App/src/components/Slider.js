import React, { Component } from 'react';


class Slider extends Component {
  render() {
    return (
      <div className="slider__wrapper">
        <button href="#" className="slider__prev"></button>
        <button href="#" className="slider__next"></button>
        <div className="slider__container">
          <ul className="slider__list">
            <li>
              A
            </li>
            <li>
              B
            </li>
            <li>
              C
            </li>
          </ul>
        </div>
      </div>
    );
  }
}

export default Slider;
