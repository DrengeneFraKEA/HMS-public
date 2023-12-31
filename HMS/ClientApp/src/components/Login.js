import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import '../styling/Login.css';
import $ from 'jquery';
import axios from 'axios';
import { Main } from './Main';

export class Login extends Component {
    static displayName = Login.name;
    submitLinkRef = React.createRef();

    componentDidMount()
    {
        // Reset everything in case cache exists.
        localStorage.setItem('token', "");
        localStorage.setItem('userid', "");
        localStorage.setItem('role', "");
    }

    handleSubmit = async (e) => {
        e.preventDefault();
        var username = $('#username').val();
        var password = $('#password').val();

        try {
            const response = await axios.post("login", {
                username: username,
                password: password
            });

            if (response.data !== "") {

                // Sucessful login. Save token, userid and role into localstorage/browser storage
                var token = "Bearer " + response.data["Token"];

                localStorage.setItem('token', token); 
                this.setState({ token });

                localStorage.setItem('userid', response.data["UserId"]);
                localStorage.setItem('role', response.data["Role"]);

                this.submitLinkRef.current.click();

            }
            else {

                alert("Ugyldigt CPR-nummer eller kodeord.");
            }
        } catch (error) {
            console.log(error);
        }
    }

    Register = async (e) =>
    {
        e.preventDefault();
        var username = $('#username').val();
        var password = $('#password').val();

        try {

            const response = await axios.post("login/register", {
                username: username,
                password: password
            });

            console.log(response.data);

            if (response.data === true) {

                alert("Du er nu oprettet i HMS.");
            }
            else if (response.data === "") {

                alert("Ugyldigt CPR eller kodeord.");

            } else {

                alert("Dette CPR-nummer findes allerede i HMS.");
            }
        } catch (error) {
            console.log(error);
        }

    }

  render() {
    return (
        <div className="login-container">
            <div className="login-box">
                <h2>HMS</h2>
                <form onSubmit={this.handleSubmit} >
                    <div className="input-group">
                        <label htmlFor="username">CPR-nummer</label>
                        <input type="text" id="username" />
                    </div>
                    <div className="input-group">
                        <label htmlFor="password">Kodeord</label>
                        <input type="password" id="password"/>
                    </div>
                    <button type="submit" id="loginbutton">Log ind</button>
                    <button onClick={this.Register}>Registrer</button>
                </form>
                <Link to="/main" className="submit-link" ref={this.submitLinkRef} />
            </div>
        </div>
      );
    }
}