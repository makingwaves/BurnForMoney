import React, { Component } from 'react';
import Modal from 'react-modal';

import './VideoHeader.css';
import logo from 'img/logo.svg';
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
        <video loop muted autoPlay poster={logo} className="VideoHeader__background">
            <source src={bfmVideo} type="video/mp4"/>
        </video>
        <div className="VideoHeader__content">
          <img src={logo} alt="Burn for money" className="VideoHeader__logo" />
          <p>workout to support charity</p>
          <button onClick = {this.openModal} >watch in action</button>
        </div>

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
