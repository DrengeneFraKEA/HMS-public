import React, { Component } from 'react';
import Navbar from "./Navbar";
import '../styling/navbar.css';
import '../styling/Main.css';
import axios from 'axios';

export class Main extends Component {
    static displayName = Main.name;

    constructor(props) {
        super(props);
        this.state = {
            userData: null
        };
    }

    componentDidMount()
    {
        // Check if user is logged in.
        var token = localStorage.getItem("token");
        if (token === "") {
            window.location.href = '/';
            return;
        }

        // Fetch person data
        try {
            var request = "person/" + localStorage.getItem('userid');

            axios.post(request, {
                headers: { "Authorization": localStorage.getItem('token') }
            })
                .then(response => {
                    this.setState({ userData: response.data });
                });
        } catch (error) {
            console.log(error);
        }
    }

    render() {
        const { userData } = this.state;

        return (
            <div>
                <Navbar />
                <div className="user-info-box">
                    <h2>Personlig information</h2>
                    {userData && (
                        <>
                            <div className="info-item">
                                <strong>Fornavn:</strong> {userData.Firstname}
                            </div>
                            <div className="info-item">
                                <strong>Efternavn:</strong> {userData.Lastname}
                            </div>
                            <div className="info-item">
                                <strong>CPR:</strong> {userData.CPR}
                            </div>
                            <div className="info-item">
                                <strong>Telefon nummer:</strong> {userData.Phonenumber}
                            </div>
                            <div className="info-item">
                                <strong>Addresse:</strong> {userData.Address}
                            </div>
                        </>
                    )}
                    <div className="info-item">
                        <strong>Rolle:</strong> {localStorage.getItem("role")}
                    </div>
                </div>
            </div>
        );
    }
}