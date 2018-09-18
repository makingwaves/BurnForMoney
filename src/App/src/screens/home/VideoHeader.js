import React, { Component } from 'react';
import Modal from 'react-modal';

import './VideoHeader.css';
import logoMW from 'img/mw-logo.svg';
import logoBFMblue from 'img/bfm-logo-blue.svg';
import awardCSR from 'img/award-csr.svg';
import arrowDown from 'img/arrow-down.svg';
import bfmVideo from 'video/BFM.mp4';

const customStyles = {
  content : {
    top                   : '50%',
    left                  : '52%',
    right                 : 'auto',
    bottom                : 'auto',
    marginRight           : '-50%',
    transform             : 'translate(-52%, -50%)'
  }
};

// setAppElement() - hide application from screenreaders and other assistive technologies while the modal is open
Modal.setAppElement('#root');

class VideoHeader extends Component {
  constructor() {
    super();

    this.state = {
      modalIsOpen: false
    };

    this.openModal = this.openModal.bind(this);
    this.afterOpenModal = this.afterOpenModal.bind(this);
    this.closeModal = this.closeModal.bind(this);
  }

  openModal() {
    this.setState({modalIsOpen: true});
  }

  afterOpenModal() {
    function updateVideoDimensions(video){
      var videoRatio = video.videoWidth / video.videoHeight,
          windowRatio = window.innerWidth / window.innerHeight;
      if(videoRatio < windowRatio){
        video.height = window.innerHeight - 100;
      } else {
        video.width = window.innerWidth - 100;
      }
    }

    var videoModal = document.querySelector('.VideoHeader__video');
    videoModal.addEventListener( "loadedmetadata", function (e) {
      updateVideoDimensions(videoModal);
    }, false );
    window.addEventListener("resize", function (e) {
      updateVideoDimensions(videoModal);
    }, false);
  }

  closeModal() {
    this.setState({modalIsOpen: false});
  }

  render() {
    return (
      <div className="VideoHeader">
        <video loop muted autoPlay className="VideoHeader__background">
            <source src={bfmVideo} type="video/mp4"/>
        </video>
        <a href="http://makingwaves.com">
          <img src={logoMW} alt="Making Waves" className="VideoHeader__logoMw" />
        </a>
        <img src={awardCSR} alt="1. miejsce Dobra Praktyka CSR" className="VideoHeader__award" />

        <div className="VideoHeader__content">
          <img src={logoBFMblue} alt="Burn for money" className="VideoHeader__logo" />
          <div className="VideoHeader__title">
            <span className="VideoHeader__title-line">Workout</span><br/>
            <span className="VideoHeader__title-line">to support</span><br/>
            <span className="VideoHeader__title-line">charity</span>
          </div>
          <button className="VideoHeader__openVideo" onClick={this.openModal} >Watch in action</button>
        </div>
        <button className="VideoHeader__goDown"></button>

        <Modal
          isOpen={this.state.modalIsOpen}
          onAfterOpen={this.afterOpenModal}
          onRequestClose={this.closeModal}
          contentLabel="Example Modal"
          style={customStyles}
        >
          <video loop muted controls autoPlay className="VideoHeader__video">
              <source src={bfmVideo} type="video/mp4"/>
          </video>
          <button className="VideoHeader__close" onClick={this.closeModal}>close</button>
        </Modal>
      </div>
    );
  }
/*
  componentDidMount(){
    this.updateVideoDimensions();
    window.addEventListener("resize", this.updateVideoDimensions.bind(this));
  }

  componentWillUnmount() {
    window.removeEventListener("resize", this.updateVideoDimensions.bind(this));
  }
*/
}

export default VideoHeader;
