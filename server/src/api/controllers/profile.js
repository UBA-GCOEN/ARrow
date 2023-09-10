import userAdminModel from "../models/userAdminModel.js"
import userStudentModel from "../models/userStudentModel.js"
import userStaffModel from "../models/userStaffModel.js"
import userFacultyModel from "../models/userFacultyModel.js"
import userVisitorModel from "../models/userVisitorModel.js"


/**
 * Route: /profile
 * Desc: get profile details 
 */
export const profile = async (req, res) => {

    res.send("Display profile page")
}


/**
 * Route: /profile/edit
 * Desc: request to edit profile
 */
export const editProfile = async (req, res) => {

    const { name, email} = req.body

    const emailDomains = [
        "@gmail.com",
        "@yahoo.com",
        "@hotmail.com",
        "@aol.com",
        "@outlook.com",
        ];

    var result = {}    
    var newResult={}
 
 
 
        //check name length
        if (name.length < 2) {
          return res
            .status(404)
            .json({ message: "Name must be atleast 2 characters long." });
        }
 
 
 
        // check email format
        if (!emailDomains.some((v) => email.indexOf(v) >= 0)) {
          return res.status(404).json({
         message: "Please enter a valid email address",
       })};


     // conditions to figure out role
    if(req.role === 'admin'){
 
       var { branch } = req.body
           
           // update userAdmin in database 
             result = await userAdminModel.updateOne({
                name,
                email,
                branch,
             });

             
         newResult = await userAdminModel.findOne({email})

    }
    else if(req.role == 'student'){
         // update userStudent in database 
            result = await userStudentModel.updateOne({
            name,
            email,
            branch,
            intrest,
            enrollNo,
            mobile
         });

         
         newResult = await userStudentModel.findOne({email})
    }
    else if(req.role == 'staff'){
        //update staff in database
            result = await userStaffModel.updateOne({
            name,
            email,
            branch,
            designation: req.designation,
            bio: req.designation,
            mobile: req.mobile
         });

         newResult = await userStaffModel.findOne({email})
    }
    else if(req.role == 'faculty'){
        //update faculty in database
        result = await userFacultyModel.updateOne({
            name,
            email,
            branch,
            subjects,
            designation,
            education,
            bio,
            intrest,
            mobile
         });

         
         newResult = await userFacultyModel.findOne({email})
    }
    else if(req.role == 'visitor'){
        //update visitor in database
        result = await userVisitorModel.updateOne({
            name,
            email,
            bio: req.bio,
         });

         newResult = await userVisitorModel.findOne({email})
    }



    if(result){

        
        res.json({
            msg: "user "+req.role+" edited successfully",
            user: newResult
    })
     }
    
}
